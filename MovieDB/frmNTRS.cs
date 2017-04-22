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
        // �v�����g�p�e�L�X�g�t�@�C���̕ۑ��p�t�H���_���A��{�ݒ�t�@�C���Őݒ肷��
        //string appconfig = System.Environment.CurrentDirectory + @"\info.ini";
        string appconfig = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\info.ini";
        string outPath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Log\";

        //���̑��A�񃍁[�J���ϐ�
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

        // �R���X�g���N�^
        public frmNTRS()
        {
            InitializeComponent();
        }

        // ���[�h���̏���
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

            // �J�E���^�[�̕\���i�f�t�H���g�̓[���j
            txtOkCount.Text = okCount.ToString();
            txtNgCount.Text = ngCount.ToString();

            // ���O�p�t�H���_�̍쐬�i�t�H���_�����݂��Ȃ��ꍇ�j
            if (!Directory.Exists(outPath)) Directory.CreateDirectory(outPath); 
        }

        // �V���A�����X�L�������ꂽ���̏���            
        private void txtModuleId_KeyDown(object sender, KeyEventArgs e)
        {
            // �G���^�[�L�[�̏ꍇ�A�e�L�X�g�{�b�N�X�̌������P�V���܂��͂Q�S���̏ꍇ�̂݁A�������s��
            if (e.KeyCode != Keys.Enter) return;
            if (txtModuleId.ReadOnly) return;
            if (txtModuleId.Text.Length != 17 && txtModuleId.Text.Length != 24) return;

            string scanTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");

            TfSQL tf = new TfSQL();
            DataTable dt = new DataTable();
            string log = string.Empty;
            string module = txtModuleId.Text;
            string mdlShort = VBS.Left(module, 17); // �P�V�P�^�ɐݒ��߂�
            string sql1;

            // �w�X�X�W�^�w�X�X�X�t�@�C�i���`�r�r�x�́A�S�Ă̂`�n�h�H���̃f�[�^���ƍ�����
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

            // ���[�^�[�t�@�C�i���`�r�r�x�ɂ��ẮA�h�m�k�h�m�d�e�X�^�[�̏������킹�ă}�b�`���O����
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

            
            // �P�D�p�X�̃v���Z�X�����擾
            var passResults = allResults.Where(r => r.judge == "PASS").Select(r => new ProcessList() { process = r.process}).OrderBy(r => r.process).ToList();
            foreach (var p in passResults) System.Diagnostics.Debug.Print(p.process);
            
            // �Q�D�P�Ɋ܂܂�Ȃ��t�F�C���̃v���Z�X�����擾
            var failResults = allResults.Where(r => r.judge == "FAIL").Select(r => new ProcessList() { process = r.process}).OrderBy(r => r.process).ToList();
            List<string> process = failResults.Select(r => r.process).Except(passResults.Select(r => r.process)).ToList();
            failResults = failResults.Where(r => process.Contains(r.process)).ToList();
            foreach (var p in failResults) System.Diagnostics.Debug.Print(p.process);

            // �R�D�P�ɂ��Q�ɂ��܂܂�Ȃ��A�e�X�g���ʂȂ��v���Z�X���擾����
            var skipResults = targetProcessCombined.Replace("'", string.Empty).Split(',').ToList().Select(r => new ProcessList() { process = r.ToString() }).OrderBy(r => r.process).ToList();
            process = skipResults.Select(r => r.process).Except(passResults.Select(r => r.process)).ToList().Except(failResults.Select(r => r.process)).ToList();
            skipResults = skipResults.Where(r => process.Contains(r.process)).ToList();
            foreach (var p in skipResults) System.Diagnostics.Debug.Print(p.process);

            // �f�B�X�v���C�p�̃v���Z�X�����X�g�����H����
            string displayPass = string.Empty;
            string displayFail = string.Empty;
            string displayAll = string.Empty;   // ���O�p
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

            // �A�v���P�[�V�����X�N���[���ɁA�e�X�g���ʂ�\������
            if (passResults.Count == targetProcessCount)
            {
                string okImagePass = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + okImageFile;
                pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
                pnlResult.BackgroundImage = System.Drawing.Image.FromFile(okImagePass);

                // �n�j�J�E���g�̉��Z
                okCount += 1;
                txtOkCount.Text = okCount.ToString();

                // ���̃��W���[���̃X�L�����ɂ��Ȃ��A�X�L�����p�e�L�X�g�{�b�N�X�̃e�L�X�g��I�����A�㏑���\�ɂ���
                txtModuleId.SelectAll();
            }
            else
            {
                string ngImagePath = System.Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + @"\NTRS Setting\images\" + ngImageFile;
                pnlResult.BackgroundImageLayout = ImageLayout.Zoom;
                pnlResult.BackgroundImage = System.Drawing.Image.FromFile(ngImagePath);

                // �m�f�J�E���g�̉��Z
                ngCount += 1;
                txtNgCount.Text = ngCount.ToString();

                // ���̃��W���[���̃X�L�������X�g�b�v����߂��A�X�L�����p�e�L�X�g�{�b�N�X�𖳌��ɂ���
                txtModuleId.ReadOnly = true;
                txtModuleId.BackColor = Color.Red; 

                // �A���[���ł̌x��
                soundAlarm();
            }
            
            // �A�v���P�[�V�����X�N���[���ƃf�X�N�g�b�v�t�H���_�̗����̗p�r�p�ɁA���t�ƃe�X�g���ʏڍו�������쐬
            log = Environment.NewLine + scanTime + "," + module + "," + displayAll;

            // �X�N���[���ւ̕\��
            txtResultDetail.Text = log.Replace(",", ",  ").Replace(Environment.NewLine, string.Empty);

            // ���O�����݁F�������t�̃t�@�C�������݂���ꍇ�͒ǋL���A���݂��Ȃ��ꍇ�̓t�@�C�����쐬�ǋL����iAppendAllText ������Ă����j
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

        // �T�u�v���V�[�W���F �e�X�^�[�p�v�g�d�q�d��̍쐬
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

        // ���Z�b�g�{�^���������̏����F �p�l���ƃe�X�g���ʃe�L�X�g�{�b�N�X���N���A���A�X�L�����p�e�L�X�g�{�b�N�X��L���ɂ���
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

        // �����̃��O�t�@�C���̃��R�[�h���𐔂���
        private void btnTodaysCount_Click(object sender, EventArgs e)
        {

        }

        // �J�E���^�[���N���A����
        private void btnSetZero_Click(object sender, EventArgs e)
        {
            okCount = 0;
            ngCount = 0;
            txtOkCount.Text = okCount.ToString();
            txtNgCount.Text = ngCount.ToString();
        }

        // �e�X�g���ʂ��i�[����N���X
        public class TestResult
        {
            public string process { get; set; }
            public string judge { get; set; }
            public string inspectdate { get; set; }
        }

        // �e�X�g���ʂ̃v���Z�X�R�[�h�݂̂��i�[����N���X
        public class ProcessList
        {
            public string process { get; set; }
        }

        // Windows API ���C���|�[�g
        [DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filepath);

        // �ݒ�e�L�X�g�t�@�C���̓ǂݍ���
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

        //MP3�t�@�C�����Đ�����
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