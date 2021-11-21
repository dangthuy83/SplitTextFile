using System.ComponentModel;
using System.IO;

namespace SplitTextFile
{
    public partial class Form1 : Form
    {
        private readonly BackgroundWorker _backgroundWorker = new();
        private int GetSLRows()
        {
            return int.Parse(TxtSoLuong.Text);
        }
        public Form1()
        {
            InitializeComponent();
            _backgroundWorker.WorkerSupportsCancellation = true;
            _backgroundWorker.WorkerReportsProgress = true;
            _backgroundWorker.DoWork += BackgroundWorker_DoWork;
            _backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            //_backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            lblPercent.Text = "";
        }
        //private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        //{
        //    //progressBar1.Invoke(new Action(() => progressBar1.Value = 0));
        //    progressBar1.Value = e.ProgressPercentage;
        //    lblPercent.Text = String.Format("processing.....{0}%", e.ProgressPercentage);
        //    lblPercent.Refresh();
        //    progressBar1.Update();
        //}
        private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                _ = MessageBox.Show("QUÁ TRÌNH CHIA FILE ĐÃ ĐƯỢC DỪNG LẠI");
            }
            else if (e.Error != null)
            {
                _ = MessageBox.Show("CHIA FILE THÀNH CÔNG");
            }
        }
        private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
        {
            try
            {
                SplitbyRow(GetSLRows());
            }
            catch (Exception ex)
            {
                _backgroundWorker.CancelAsync();
                _ = MessageBox.Show("CHIA FILE KHÔNG THÀNH CÔNG" + Environment.NewLine + ex, "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void BtnBrowser_Click(object sender, EventArgs e)
        {
            lblPercent.Text = "";
            Openfile();
        }
        private void BtnStart_Click(object sender, EventArgs e)
        {
            if (TxtPath.Text == "" || TxtSoLuong.Text == "")
            {
                _ = MessageBox.Show("VUI LÒNG CHỌN ĐƯỜNG DẪN ĐẾN FILE TEXT", "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (!_backgroundWorker.IsBusy)
                {
                    _backgroundWorker.RunWorkerAsync();
                }
            }
        }
        private void BtnStop_Click(object sender, EventArgs e)
        {
            if (_backgroundWorker.IsBusy)
            {
                _backgroundWorker.CancelAsync();
            }
        }
        private void BtnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        private static int Lamtron(int sobichia, int sochia)
        {
            int kq;
            if (sobichia % sochia != 0 )
            {
                kq = ((sobichia / sochia) + 1); 
            }
            else
            {
                kq = sobichia / sochia;
            }
            return kq;
        }
        private void SplitbyRow(int SLRows)
        {
            using StreamReader reader = File.OpenText(TxtPath.Text);
            string[] lines = File.ReadAllLines(TxtPath.Text);
            int outFileNumber = 1;
            int index = 1;
            int totalFile = Lamtron(lines.Length, SLRows);
            string savePath = Path.GetDirectoryName(TxtPath.Text) + "/" + "SPLIT_FILE_" + Path.GetFileNameWithoutExtension(TxtPath.Text);
            if(!Directory.Exists(savePath))
            {
                _ = Directory.CreateDirectory(savePath);
            }
            while (!reader.EndOfStream)
            {
                var outFileName = savePath + "/" + Path.GetFileNameWithoutExtension(TxtPath.Text) + "_" + outFileNumber.ToString(format: "D3") + Path.GetExtension(TxtPath.Text);
                using StreamWriter writer = File.CreateText(string.Format(outFileName, outFileNumber++));
                progressBar1.Invoke(new Action(() =>
                {
                    progressBar1.Maximum = totalFile;
                    lblPercent.Text = string.Format("Processing.....{0}/{1}", index++, totalFile);
                    progressBar1.Increment(index);
                }));
                //_backgroundWorker.ReportProgress(index++ * 100 / MaxRows);
                for (int i = 0; i < SLRows; i++)
                {
                    if (!_backgroundWorker.CancellationPending)
                    {
                        //_backgroundWorker.ReportProgress(index++ * 100 / MaxRows);
                        writer.WriteLine(reader.ReadLine());
                        if (reader.EndOfStream)
                        {
                            break;
                        }
                    }
                }
                writer.Close();
            }
            reader.Close();
        }
        private void Openfile()
        {
            using OpenFileDialog openFileDialog = new() { Filter = "Text File(*.txt)|*.txt|All File(*.*)|*.*", Multiselect = false };
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                TxtPath.Text = openFileDialog.FileName;
            }
        }        
    }
}