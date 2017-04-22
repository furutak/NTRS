using System;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Globalization;
using System.Security.Permissions;
using System.Runtime.InteropServices;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

namespace NTRS
{
    public partial class frmNTRS : Form
    {
        // プリント用テキストファイルの保存用フォルダを、基本設定ファイルで設定する
        //string appconfig = System.Environment.CurrentDirectory + @"\info.ini";
        string appconfig = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\info.ini";
        string outPath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Log\";

        //その他、非ローカル変数
        int okCount;
        int ngCount;
        int targetProcessCount;
        bool sound;
        string model;
        string subAssyName;
        string targetProcess;
        string targetProcessCrust;
        string targetProcessCombined;
        string okImageFile;
        string ngImageFile;
        string standByImageFile;
        string ntrsSwitch;
        string bracketCrustLinkSwitch;
        string headTableThisMonth = string.Empty;
        string headTableLastMonth = string.Empty;

        // コンストラクタ
        public frmNTRS()
        {
            InitializeComponent();
        }

        // ロード時の処理
        private void frmModule_Load(object sender, EventArgs e)
        {
            okImageFile = readIni("MODULE-DATA MATCHING", "OK IMAGE FILE", appconfig);
            ngImageFile = readIni("MODULE-DATA MATCHING", "NG IMAGE FILE", appconfig);
            standByImageFile = readIni("MODULE-DATA MATCHING", "STAND-BY IMAGE FILE", appconfig);
            string standByImagePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + standByImageFile;
            pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
            pnlResult.BackgroundImage = System.Drawing.Image.FromFile(standByImagePath);

            model = readIni("MODULE-DATA MATCHING", "MODEL", appconfig);
            subAssyName = readIni("MODULE-DATA MATCHING", "SUB ASSY NAME", appconfig);
            targetProcess = readIni("MODULE-DATA MATCHING", "TARGET PROCESS", appconfig);
            ntrsSwitch = readIni("MODULE-DATA MATCHING", "NTRS INLINE MATCHING", appconfig);
            bracketCrustLinkSwitch = readIni("MODULE-DATA MATCHING", "BRACKET-CRUST LINK", appconfig);
            targetProcessCrust = readIni("MODULE-DATA MATCHING", "CRUST TARGET PROCESS", appconfig);

            headTableThisMonth = model.ToLower() + DateTime.Today.ToString("yyyyMM");
            headTableLastMonth = model.ToLower() + ((VBS.Right(DateTime.Today.ToString("yyyyMM"), 2) != "01") ?
                (long.Parse(DateTime.Today.ToString("yyyyMM")) - 1).ToString() : (long.Parse(DateTime.Today.ToString("yyyy")) - 1).ToString() + "12");

            txtSubAssy.Text = model + "  " + subAssyName;
            targetProcessCount = targetProcess.Where(c => c == ',').Count() + 1 + 
                (targetProcessCrust == string.Empty ? 0 : targetProcessCrust.Where(c => c == ',').Count() + 1);
            targetProcessCombined = targetProcess + (targetProcessCrust == string.Empty ? string.Empty : "," + targetProcessCrust);

            // カウンターの表示（デフォルトはゼロ）
            txtOkCount.Text = okCount.ToString();
            txtNgCount.Text = ngCount.ToString();

            // ログ用フォルダの作成（フォルダが存在しない場合）
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath); 
        }

        // シリアルがスキャンされた時の処理            
        private void txtModuleId_KeyDown(object sender, KeyEventArgs e)
        {
            // エンターキーの場合、テキストボックスの桁数が１７桁または２４桁の場合のみ、処理を行う
            if (e.KeyCode != Keys.Enter) return;
            if (txtModuleId.ReadOnly) return;
            if (txtModuleId.Text.Length != 17 && txtModuleId.Text.Length != 24) return;

            string scanTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            TfSQL tf = new TfSQL();
            DataTable dt = new DataTable();
            string log = string.Empty;
            string module = txtModuleId.Text;
            string mdlShort = VBS.Left(module, 17); // １７ケタに設定を戻す
            string sql1;

            // Ｘ９９８／Ｘ９９９ファイナルＡＳＳＹは、全てのＡＯＩ工程のデータを照合する
            if (bracketCrustLinkSwitch == "ON")
            {
                sql1 = "select process, judge, max(inspectdate) as inspectdate from (" +
                    "(select process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableThisMonth + 
                        " where (process in (" + targetProcess + ") and serno = '" + mdlShort + "') " +
                            "or (process in (" + targetProcessCrust + ") and (" + 
                                "serno in (select lot from " + headTableThisMonth + "tbi where serno = '" + mdlShort + "' and lot != 'null') or " +
                                "serno in (select lot from " + headTableLastMonth + "tbi where serno = '" + mdlShort + "' and lot != 'null') ) ) )" +
                    "union all " +
                    "(select process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableLastMonth +
                        " where (process in (" + targetProcess + ") and serno = '" + mdlShort + "') " +
                            "or (process in (" + targetProcessCrust + ") and (" +
                                "serno in (select lot from " + headTableLastMonth + "tbi where serno = '" + mdlShort + "' and lot != 'null') ) ) )" +
                    ") d group by judge, process order by judge desc, process";
            }
            else
            {
                sql1 = "select process, judge, max(inspectdate) as inspectdate from (" +
                    "(select process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableThisMonth + " where process in (" + targetProcess + ") and serno = '" + mdlShort + "') union all " +
                    "(select process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableLastMonth + " where process in (" + targetProcess + ") and serno = '" + mdlShort + "')" +
                    ") d group by judge, process order by judge desc, process";
            }
            System.Diagnostics.Debug.Print(sql1);
            tf.sqlDataAdapterFillDatatableFromPqmDb(sql1, ref dt);

            // モーターファイナルＡＳＳＹについては、ＩＮＬＩＮＥテスターの情報もあわせてマッチングする
            if (ntrsSwitch == "ON")
            {
                string sql2 = "select process, judge, max(inspectdate) as inspectdate from (" +
                    "(select 'BOJAY' as process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableThisMonth + " where serno = '" + mdlShort + "' and factory = 'SLEF') union all " +
                    "(select 'BOJAY' as process, case when tjudge = '0' then 'PASS' else 'FAIL' end as judge, inspectdate from " + headTableLastMonth + " where serno = '" + mdlShort + "' and factory = 'SLEF')" +
                    ") d group by judge, process order by judge desc, process";
                System.Diagnostics.Debug.Print(sql2);
                tf.sqlDataAdapterFillDatatableFromTesterDb(sql2, ref dt);
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                System.Diagnostics.Debug.Print(dt.Rows[i][0].ToString() + " " + dt.Rows[i][1].ToString() + " " + dt.Rows[i][2].ToString());
            }

            var allResults = dt.AsEnumerable().Select(r => new TestResult()
            { process = r.Field<string>("process"), judge = r.Field<string>("judge"), inspectdate = r.Field<DateTime>("inspectdate").ToString("yyyy/MM/dd HH:mm:ss"), }).ToList();

            
            // １．パスのプロセス名を取得
            var passResults = allResults.Where(r => r.judge == "PASS").Select(r => new ProcessList() { process = r.process}).OrderBy(r => r.process).ToList();
            foreach (var p in passResults) System.Diagnostics.Debug.Print(p.process);
            
            // ２．１に含まれないフェイルのプロセス名を取得
            var failResults = allResults.Where(r => r.judge == "FAIL").Select(r => new ProcessList() { process = r.process}).OrderBy(r => r.process).ToList();
            List<string> process = failResults.Select(r => r.process).Except(passResults.Select(r => r.process)).ToList();
            failResults = failResults.Where(r => process.Contains(r.process)).ToList();
            foreach (var p in failResults) System.Diagnostics.Debug.Print(p.process);

            // ３．１にも２にも含まれない、テスト結果なしプロセスを取得する
            var skipResults = targetProcessCombined.Replace("'", string.Empty).Split(',').ToList().Select(r => new ProcessList() { process = r.ToString() }).OrderBy(r => r.process).ToList();
            process = skipResults.Select(r => r.process).Except(passResults.Select(r => r.process)).ToList().Except(failResults.Select(r => r.process)).ToList();
            skipResults = skipResults.Where(r => process.Contains(r.process)).ToList();
            foreach (var p in skipResults) System.Diagnostics.Debug.Print(p.process);

            // ディスプレイ用のプロセス名リストを加工する
            string displayPass = string.Empty;
            string displayFail = string.Empty;
            string displayAll = string.Empty;   // ログ用
            List<TestResult> allLog = new List<TestResult>();
            foreach (var p in passResults)
            {
                displayPass += p.process + " ";
                allLog.Add(new TestResult { process = p.process, judge = "PASS", inspectdate = string.Empty });
            }
            displayPass = displayPass.Trim();
            foreach (var p in failResults)
            {
                displayFail += p.process + " F ";
                allLog.Add(new TestResult { process = p.process, judge = "FAIL", inspectdate = string.Empty });
            }
            foreach (var p in skipResults)
            {
                displayFail += p.process + " S ";
                allLog.Add(new TestResult { process = p.process, judge = "SKIP", inspectdate = string.Empty });
            }
            displayFail = displayFail.Trim();
            allLog = allLog.OrderBy(r => r.process).ToList();
            foreach (var p in allLog)
            {
                displayAll += (p.process + ":" + p.judge + ",");
            }
            displayAll = VBS.Left(displayAll, displayAll.Length - 1);

            // アプリケーションスクリーンに、テスト結果を表示する
            if (passResults.Count == targetProcessCount)
            {
                string okImagePass = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + okImageFile;
                pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
                pnlResult.BackgroundImage = System.Drawing.Image.FromFile(okImagePass);

                // ＯＫカウントの加算
                okCount += 1;
                txtOkCount.Text = okCount.ToString();

                // 次のモジュールのスキャンにそなえ、スキャン用テキストボックスのテキストを選択し、上書き可能にする
                txtModuleId.SelectAll();
            }
            else
            {
                string ngImagePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + ngImageFile;
                pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
                pnlResult.BackgroundImage = System.Drawing.Image.FromFile(ngImagePath);

                // ＮＧカウントの加算
                ngCount += 1;
                txtNgCount.Text = ngCount.ToString();

                // 次のモジュールのスキャンをストップするめた、スキャン用テキストボックスを無効にする
                txtModuleId.ReadOnly = true;
                txtModuleId.BackColor = Color.Red; 

                // アラームでの警告
                soundAlarm();
            }
            
            // アプリケーションスクリーンとデスクトップフォルダの両方の用途用に、日付とテスト結果詳細文字列を作成
            log = Environment.NewLine + scanTime + "," + module + "," + displayAll;

            // スクリーンへの表示
            txtResultDetail.Text = log.Replace(",", ",  ").Replace(Environment.NewLine, string.Empty);

            // ログ書込み：同日日付のファイルが存在する場合は追記し、存在しない場合はファイルを作成追記する（AppendAllText がやってくれる）
            try
            {
                string outFile = outPath + DateTime.Today.ToString("yyyyMMdd") + ".txt";
                if (System.IO.File.Exists(outFile))
                {
                    System.IO.File.AppendAllText(outFile, log, System.Text.Encoding.GetEncoding("UTF-8"));
                }
                else
                {
                    string header = DateTime.Today.ToString("yyyy/MM/dd") + " " + model + " " + subAssyName + 
                        Environment.NewLine + "SCAN TIME,PRODUCT SERIAL,TEST DETAIL";
                    System.IO.File.AppendAllText(outFile, header + log, System.Text.Encoding.GetEncoding("UTF-8"));
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // サブプロシージャ： テスター用ＷＨＥＲＥ句の作成
        //private string makeSqlWhereClause(string criteria)
        //{
        //    string sql = " where (";
        //    foreach (string c in criteria.Split(','))
        //    {
        //        sql += "process = " + c + " or ";
        //    }
        //    sql = VBS.Left(sql, sql.Length - 3) + ") ";
        //    System.Diagnostics.Debug.Print(sql);
        //    return sql;
        //}

        // リセットボタン押下時の処理： パネルとテスト結果テキストボックスをクリアし、スキャン用テキストボックスを有効にする
        private void btnReset_Click(object sender, EventArgs e)
        {
            string standByImagePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + standByImageFile;
            pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
            pnlResult.BackgroundImage = System.Drawing.Image.FromFile(standByImagePath);

            txtResultDetail.Text = string.Empty;
            txtModuleId.Text = string.Empty;
            txtModuleId.ReadOnly = false;
            txtModuleId.BackColor = Color.White;
            txtModuleId.Focus();
        }

        // 当日のログファイルのレコード数を数える
        private void btnTodaysCount_Click(object sender, EventArgs e)
        {

        }

        // カウンターをクリアする
        private void btnSetZero_Click(object sender, EventArgs e)
        {
            okCount = 0;
            ngCount = 0;
            txtOkCount.Text = okCount.ToString();
            txtNgCount.Text = ngCount.ToString();
        }

        // テスト結果を格納するクラス
        public class TestResult
        {
            public string process { get; set; }
            public string judge { get; set; }
            public string inspectdate { get; set; }
        }

        // テスト結果のプロセスコードのみを格納するクラス
        public class ProcessList
        {
            public string process { get; set; }
        }

        // Windows API をインポート
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filepath);

        // 設定テキストファイルの読み込み
        private string readIni(string s, string k, string cfs)
        {
            StringBuilder retVal = new StringBuilder(255);
            string section = s;
            string key = k;
            string def = String.Empty;
            int size = 255;
            int strref = GetPrivateProfileString(section, key, def, retVal, size, cfs);
            return retVal.ToString();
        }

        //MP3ファイルを再生する
        [System.Runtime.InteropServices.DllImport("winmm.dll")]
        private static extern int mciSendString(String command,
           StringBuilder buffer, int bufferSize, IntPtr hwndCallback);

        private string aliasName = "MediaFile";

        private void soundAlarm()
        {
            string currentDir = System.Environment.CurrentDirectory;
            string fileName = currentDir + @"\warning.mp3";
            string cmd;

            if (sound)
            {
                cmd = "stop " + aliasName;
                mciSendString(cmd, null, 0, IntPtr.Zero);
                cmd = "close " + aliasName;
                mciSendString(cmd, null, 0, IntPtr.Zero);
                sound = false;
            }

            cmd = "open \"" + fileName + "\" type mpegvideo alias " + aliasName;
            if (mciSendString(cmd, null, 0, IntPtr.Zero) != 0) return;
            cmd = "play " + aliasName;
            mciSendString(cmd, null, 0, IntPtr.Zero);
            sound = true;
        }
    }
}