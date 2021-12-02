using System;
using System.Windows.Forms;
using ZI___Zadatak_1_od_3.PlayFairCipher;
using System.IO;
using System.Text.RegularExpressions;
using ZI___Zadatak_1_od_3.AES;
using System.Text;
using System.Security.Cryptography;

namespace ZI___Zadatak_1_od_3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Encrypt(string srcPath, string file, string dstPath, string key)
        {
            string txtToEncrypt = File.ReadAllText(srcPath);
            string encryptedTxt = PlayfairCipher.Encipher(txtToEncrypt, key);

            File.AppendAllText(textBox3.Text, file + " " + key + Environment.NewLine);
            File.WriteAllText(dstPath + @"\" + file, encryptedTxt);

        }

        private void AESEncrypt(string srcPath, string file, string dstPath)
        {
            using (AesManaged AES = new AesManaged())
            {  
                string txtToEncrypt = File.ReadAllText(srcPath);
                byte[] encryptedBytes = aes.EncryptStringToBytes_Aes(txtToEncrypt, AES.Key, AES.IV);
                string key = Convert.ToBase64String(AES.Key);
                string vector = Convert.ToBase64String(AES.IV); 
                string encryptedTxt = Convert.ToBase64String(encryptedBytes);

                File.AppendAllText(textBox3.Text, file + " " + key + " " + vector + Environment.NewLine);

                File.WriteAllText(dstPath + @"\" + file, encryptedTxt);
            }
        }

        private void Decrypt(string srcPath, string file, string destPath)
        {
            string encryptedTxt = File.ReadAllText(srcPath);
            foreach (string line in File.ReadLines(textBox3.Text))
            {

                if (line.StartsWith(file))
                {
                    string[] key = line.Split(" ");
                    string txtToDecrypt = PlayfairCipher.Decipher(encryptedTxt, key[1]);
                    File.WriteAllText(destPath + @"\" + file, txtToDecrypt);
                }
            }
        }

        private void AESDecrypt(string srcPath, string file, string dstPath)
        {
            using (AesManaged AES = new AesManaged())
            {
                string decrypted = null;
                string pom = null;
                string[] encryptedTxt = File.ReadAllText(srcPath).Split(" ");
                byte[] encryptedBytes = null;
                foreach (string line in File.ReadAllLines(textBox3.Text))
                {
                    string[] key;
                    if (line.StartsWith(file))
                    {
                        key = line.Split(" ");
                        byte[] k = Convert.FromBase64String(key[1]);
                        byte[] v = Convert.FromBase64String(key[2]);

                        if (key[1] != null && key[2] != null)
                        {
                            foreach (string str in encryptedTxt)
                            {
                                if (str == "")
                                {
                                    continue;
                                }

                                pom += str;
                            }
                            encryptedBytes = Convert.FromBase64String(pom);
                            decrypted = aes.DecryptStringFromBytes_Aes(encryptedBytes, k, v);
                            break;
                        }

                    }
                }

                File.WriteAllText(dstPath + @"\" + file, decrypted);
            }
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            textBox1.Text = @"C:\Users\Mateja\source\repos\ZI - Zadatak 1 od 3\Enkriptovani";
            textBox2.Text = @"C:\Users\Mateja\source\repos\ZI - Zadatak 1 od 3\Dekriptovani";
            textBox3.Text = @"C:\Users\Mateja\source\repos\ZI - Zadatak 1 od 3\Kljuc.txt";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = fbd.SelectedPath;
                }
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                if (fbd.ShowDialog() == DialogResult.OK)
                {
                    textBox2.Text = fbd.SelectedPath;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                button1.Enabled = false;
                textBox1.Enabled = false;
                button2.Enabled = false;
                textBox2.Enabled = false;
                button3.Enabled = false;
                textBox3.Enabled = false;
                button4.Enabled = false;
                button5.Enabled = false;
            }
            else
            {
                textBox1.Enabled = true;
                textBox2.Enabled = true;
                button3.Enabled = true;
                textBox3.Enabled = true;
                button4.Enabled = true;
                button5.Enabled = true;

                if (PlayfairCipherBtn.Checked || AESBtn.Checked)
                {
                    button1.Enabled = true;
                    button2.Enabled = true;
                }
            }
        }

        private void FileWatcher_Created(object sender, System.IO.FileSystemEventArgs e)
        {
            if (checkBox1.Checked)
            {
                string dstFile = textBox1.Text;

                if (PlayfairCipherBtn.Checked)
                {
                    Encrypt(e.FullPath, e.Name, dstFile, textBox5.Text);
                }
                else if (AESBtn.Checked)
                {
                    AESEncrypt(e.FullPath, e.Name, dstFile);
                }          
                else
                {
                    MessageBox.Show("Izaberite nacin enkripcije");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (new FileInfo(ofd.FileName).Length == 0)
                    {
                        return;
                    }

                    if (PlayfairCipherBtn.Checked)
                    {
                        Encrypt(ofd.FileName, Path.GetFileName(ofd.FileName), textBox1.Text, textBox5.Text);
                    }
                    else
                    {
                        AESEncrypt(ofd.FileName, Path.GetFileName(ofd.FileName), textBox1.Text);
                    }
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    if (new FileInfo(ofd.FileName).Length == 0)
                    {
                        return;
                    }

                    if (PlayfairCipherBtn.Checked)
                    {
                        Decrypt(ofd.FileName, Path.GetFileName(ofd.FileName), textBox2.Text);
                    }
                    else
                    {
                        AESDecrypt(ofd.FileName, Path.GetFileName(ofd.FileName), textBox2.Text);
                    }
                    
                }
            }
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            textBox5.Text = Regex.Replace(textBox5.Text, " ", "");

            string text = textBox5.Text;

            for (int i = 0; i < textBox5.Text.Length; i++)
            {
                
                if (!char.IsLetter(text[i]))
                {
                    MessageBox.Show("Unesite samo slova");
                    textBox5.Text = textBox5.Text.Remove(textBox5.Text.Length - 1);
                }
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    textBox3.Text = ofd.FileName;
                }
            }
        }

        private void PlayfairCipherBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (PlayfairCipherBtn.Checked)
            {
                textBox5.Enabled = true;
                textBox5.Show();
                button1.Enabled = true;
                button2.Enabled = true;

                if (checkBox1.Checked)
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                }
            }
        }

        private void AESBtn_CheckedChanged(object sender, EventArgs e)
        {
            if (AESBtn.Checked)
            {
                textBox5.Enabled = false;
                button1.Enabled = true;
                button2.Enabled = true;

                if (checkBox1.Checked)
                {
                    button1.Enabled = false;
                    button2.Enabled = false;
                }
            }
        }
    }
}
