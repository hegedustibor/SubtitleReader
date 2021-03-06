﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Net;

using System.Threading;

namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {
        private List<string> container;
        private List<string> containerHun;
        private string translateWord = null;

        public Form1()
        {
            InitializeComponent();

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.ShowDialog();
            textBox1.Text = openFileDialog.FileName;
        }

        private async void button3_Click(object sender, EventArgs e)
        {

            string line;
            string[] words = null;
            container = new List<string>();
            containerHun = new List<string>();
            int i = 0;
            StreamReader streamReader = new StreamReader(textBox1.Text);

            while((line = streamReader.ReadLine()) != null) {
   
                
                if ( Regex.IsMatch(line, @"\d"))
                {
                    //Console.WriteLine("ok");
                    continue;
                }

                string fixedInput = Regex.Replace(line, "[^a-zA-Z0-9% -]", string.Empty);
                string cleartext = Regex.Replace(fixedInput, "\r\n", string.Empty);
                words = fixedInput.Split(' ');
                
                foreach (string part in words)
                {
                    if (!part.Equals(""))
                    {
                        if (!container.Contains(part.ToString()))
                        {
                            container.Add(part);
                            await Translate(part, "en", "hu");
                            containerHun.Add(this.translateWord);
                            richTextBox1.AppendText(part + "\t - \t" + this.translateWord + '\n');
                        } else
                        {
                            //Console.WriteLine("Benne van.");
                        }
                        
                    }

                }
                
                //richTextBox1.AppendText(words.ToString() + "\n");
            }
            streamReader.Close();
            
            

        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Text Files | *.txt";
            saveFileDialog.DefaultExt = "txt";
            saveFileDialog.Title = "Mentsd el egy szöveges fájlba...";
            saveFileDialog.ShowDialog();

            if (saveFileDialog.FileName != "")
            {
                FileStream fileStream = (FileStream)saveFileDialog.OpenFile();
                StreamWriter streamWriter = new StreamWriter(fileStream);

                foreach(string word in container)
                {
                    streamWriter.WriteLine(word + "\n");
                }
                streamWriter.Flush();
                streamWriter.Close();
                fileStream.Close();
            }
        }

        private Task Translate(string text, string from, string to)
        {
            return Task.Factory.StartNew(() => {
                try
                {
                    WebClient wc = new WebClient();
                    wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                    wc.Headers.Add(HttpRequestHeader.AcceptCharset, "UTF-8");
                    wc.Encoding = Encoding.UTF8;

                    string url = string.Format(@"http://translate.google.com.tr/m?hl=en&sl={0}&tl={1}&ie=UTF-8&prev=_m&q={2}",
                                                from, to, Uri.EscapeUriString(text));

                    this.translateWord = wc.DownloadString(url);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    //return null;
                }

                this.translateWord = this.translateWord.Remove(0, this.translateWord.IndexOf("<div dir=\"ltr\" class=\"t0\">")).Replace("<div dir=\"ltr\" class=\"t0\">", "");
                int last = this.translateWord.IndexOf("</div>");
                this.translateWord = this.translateWord.Remove(last, this.translateWord.Length - last);
            });
            

        }


    }
}
