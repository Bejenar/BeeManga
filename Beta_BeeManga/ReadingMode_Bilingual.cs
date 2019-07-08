using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Beta_BeeManga
{
    public partial class ReadingMode_Bilingual : Form
    {
        bool first = false;
        Manga manga_ru, manga_jp;
        bool flip = false;
        string dir = @"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Reading\";

        private void button3_Click(object sender, EventArgs e)
        {
            if (flip)
            {
                if(File.Exists(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex + 1).ToString() + ".jpg"))
                webBrowser1.Navigate(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex+1).ToString() + ".jpg");
                else
                    webBrowser1.Navigate(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex + 1).ToString() + ".png");

            }
            else
            {
                if(File.Exists(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".jpg"))
                webBrowser1.Navigate(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".jpg");
                else
                    webBrowser1.Navigate(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".png");
            }

            flip = !flip;

        }

        private void comboBox3_DropDownClosed(object sender, EventArgs e)
        {
            
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(dir);
            Chapter c1 = Chapter.ByTitle(manga_jp, Convert.ToString(comboBox3.SelectedItem));
            Chapter c2 = Chapter.ByTitle(manga_ru, Convert.ToString(comboBox4.SelectedItem));
            if (first)
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    if (!file.Name.StartsWith(c1.title) && !file.Name.StartsWith(c2.title))
                        file.Delete();
                }
            }
            else { first = true; }

            manga_ru.helper.DownloadChapter(c2);
            comboBox2.Items.Clear();
            for (int i = 1; i <= c2.pageUrl.Count; i++)
                comboBox2.Items.Add(i);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            
            System.IO.DirectoryInfo di = new DirectoryInfo(dir);
            Chapter c1 = Chapter.ByTitle(manga_jp, Convert.ToString(comboBox3.SelectedItem));
            Chapter c2 = Chapter.ByTitle(manga_ru, Convert.ToString(comboBox4.SelectedItem));

            if (first)
            {
                foreach (FileInfo file in di.EnumerateFiles())
                {
                    if (!file.Name.StartsWith(c1.title) && !file.Name.StartsWith(c2.title))
                        file.Delete();
                }
                
            }
            else { first = true; }
            manga_jp.helper.DownloadChapter(c1);
            comboBox1.Items.Clear();
            for (int i = 1; i <= c1.pageUrl.Count; i++)
                comboBox1.Items.Add(i);

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if(comboBox2.SelectedIndex + 1 < comboBox2.Items.Count)
            comboBox2.SelectedIndex += 1;
            if (comboBox1.SelectedIndex + 1 < comboBox1.Items.Count)
                comboBox1.SelectedIndex += 1;


        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".jpg"))
                webBrowser1.Navigate(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".jpg");
            else
                webBrowser1.Navigate(dir + Convert.ToString(comboBox4.SelectedItem) + "_" + (comboBox2.SelectedIndex + 1).ToString() + ".png");
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (File.Exists(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex + 1).ToString() + ".jpg"))
                webBrowser1.Navigate(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex + 1).ToString() + ".jpg");
            else
                webBrowser1.Navigate(dir + Convert.ToString(comboBox3.SelectedItem) + "_" + (comboBox1.SelectedIndex + 1).ToString() + ".png");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex - 1 >= 0)
                comboBox2.SelectedIndex -= 1;
            if (comboBox1.SelectedIndex - 1 >= 0)
                comboBox1.SelectedIndex -= 1;
        }

        public ReadingMode_Bilingual()
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            //webBrowser1.Navigate(@"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\Dr. Stone - Raw Chapter 111_1.jpg");
            manga_ru = Program.manga_ru;
            manga_jp = Program.manga_jp;
            

            manga_jp.helper.GetChaptersList(manga_jp.url);
            manga_ru.helper.GetChaptersList(manga_ru.url);

            manga_jp.chapters = manga_jp.helper.getChapters();
            manga_ru.chapters = manga_ru.helper.getChapters();

            for(int i = 0; i < manga_jp.chapters.Count; i++)
            {
                comboBox3.Items.Add(manga_jp.chapters[i].title);
            }

            for (int i = 0; i < manga_ru.chapters.Count; i++)
            {
                comboBox4.Items.Add(manga_ru.chapters[i].title);
            }


            //manga_jp.helper.DownloadChapter(manga_jp.chapters[0]);
            //manga_ru.helper.DownloadChapter(manga_ru.chapters[0]);

            //webBrowser1.Navigate(@"C:\Users\jion9\source\repos\BeeManga\Beta_BeeManga\bin\Debug\" + manga_jp.chapters[0].title + "_1.jpg");
            //pictureBox1. = new Point(this.Size.Height, 3000);
            
        }

        
    }
}
