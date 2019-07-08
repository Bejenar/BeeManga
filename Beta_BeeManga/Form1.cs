using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms; 
namespace Beta_BeeManga
{
    public partial class Form1 : Form
    {
        Thread searchThread = new Thread(Searcher);
        //List<Manga> mangas = new List<Manga>();

        List<PictureBox> pic = new List<PictureBox>();
        List<Label> lab = new List<Label>();
        List<Button> buttons = new List<Button>();
        private static IHtmlParser helper; 

        public Form1()
        {
            InitializeComponent();
            comboBox1.Items.Add(new extenshion_lhscan());
            comboBox1.Items.Add(new extenshion_readmanga());
        }

        delegate void Del();

        public static void Searcher(object sender)
        {
            Form1 form = (Form1)sender;
            form.Invoke(new Del(() => form.AutoScroll = false));
            //IHtmlParser manga = new extenshion_lhscan();
            IHtmlParser manga = null;
            form.Invoke(new MethodInvoker(() => manga = (IHtmlParser)form.comboBox1.SelectedItem));

            List<Manga> buffer = new List<Manga>();

            manga.SearchByName(form.textBox1.Text);

            List<string> searchResultTitles = manga.getTitles();
            List<string> searchResultUrls = manga.getUrls();
            List<string> searchResultImg = manga.getImg();

            int y = 65;
            for (int i = 0; i < searchResultTitles.Count; i++)
            {
                Manga buf = new Manga(searchResultTitles[i], searchResultUrls[i], searchResultImg[i], manga);
                form.pic.Add(new PictureBox());
                form.lab.Add(new Label());
                form.buttons.Add(new Button());
                    
                    form.pic[i].SetBounds(40, y, 85, 110);
                    form.lab[i].SetBounds(150, y, 300, 40);
                    form.buttons[i].SetBounds(460, y, 20, 20);
                    y += 130;

                form.pic[i].SizeMode = PictureBoxSizeMode.StretchImage;
                //form.pic[i].Tag = searchResultUrls[i];
                form.pic[i].Tag = buf;
                form.pic[i].Click += new EventHandler(Link);

                form.buttons[i].Text = i.ToString();
                form.buttons[i].Click += new EventHandler(SelectManga);
                form.Invoke(new Del(() => form.Controls.Add(form.buttons[i])));

                try
                {
                    WebRequest req = WebRequest.Create(searchResultImg[i]);
                    WebResponse response = req.GetResponse();
                    Stream stream = response.GetResponseStream();
                    form.pic[i].Image = Image.FromStream(stream);
                }
                catch
                {
                    form.pic[i].Image = Image.FromFile("default.jpg");
                }

                form.Invoke(new Del(() => form.Controls.Add(form.pic[i]))); //

                //form.lab[i].AutoSize = true;
                form.lab[i].Text = searchResultTitles[i];

                form.Invoke(new Del(() => form.Controls.Add(form.lab[i]))); //

            }
            form.Invoke(new Del(() => form.AutoScroll = true));

            //form.Invoke(new Del(() => form.mangas = buffer));

        }

        private static void SelectManga(object sender, EventArgs e)
        {
            Button button = (Button)sender;
            Manga currentManga = (Manga)Program.form.pic[Convert.ToInt32(button.Text)].Tag;

            if (Program.manga_jp == null)
                Program.manga_jp = currentManga;
            else
            {
                Program.manga_ru = currentManga;
                ReadingMode_Bilingual form = new ReadingMode_Bilingual();
                form.Show();
            }

            Console.WriteLine("selected");
        }

        private static void SwitchControls(bool hide, Form1 form)
        {
            if (hide)
            {
                foreach(Control c in form.Controls)
                {
                    c.Hide();
                }
                form.panel1.Visible = true;
            }
            else
            {
                foreach (Control c in form.Controls)
                {
                    c.Show();
                }
                form.panel1.Visible = false;
            }
        }

        private static void setPanel(Manga manga, Form1 form)
        {
            helper = manga.helper;
            try
            {
                WebRequest req = WebRequest.Create(manga.thumbnailUrl);
                WebResponse response = req.GetResponse();
                Stream stream = response.GetResponseStream();
                form.pictureBox1.Image = Image.FromStream(stream);
            }
            catch
            {
                form.pictureBox1.Image = Image.FromFile("default.jpg");
            }
            /// title
            form.label1.Text = manga.title;

            int y = 10;
            int k = manga.chapters.Count;
            foreach (Chapter chapter in manga.chapters)
            {
                Label label = new Label();
                label.AutoSize = true;
                label.Text = chapter.title + " " + chapter.time;
                label.Location = new Point(0, y);
                label.Tag = chapter;
                label.Click += new System.EventHandler(DownloadChapter);
                y += 40;
                form.panel2.Controls.Add(label);
            }
        }

        private static void DownloadChapter(object sender, EventArgs e)
        {
            Label label = (Label)sender;
            Chapter chapter = (Chapter)label.Tag;
            helper.DownloadChapter(chapter);
        }

        private static void Link(object sender, EventArgs e)
        {
            PictureBox pictureBox = (PictureBox)sender;
            Manga currentManga = (Manga)pictureBox.Tag;
            currentManga.helper.GetChaptersList(currentManga.url);
            currentManga.chapters = currentManga.helper.getChapters();

            

            SwitchControls(true, Program.form);
            setPanel(currentManga, Program.form);


            /*MangaInfoForm mangaInfoForm = new MangaInfoForm(currentManga);
            mangaInfoForm.Show();
            mangaInfoForm.BringToFront();*/


            /*Console.WriteLine(currentManga.title);

            currentManga.helper.GetChaptersList(currentManga.url);
            List<string> chapters = currentManga.helper.getChapters();
            foreach(var c in chapters)
            {
                Console.WriteLine(c);
            }*/


            //Process.Start(Convert.ToString(pictureBox.Tag));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            foreach (var c in Controls.OfType<Button>())
            {
                if(!c.Equals(button1))
                    Controls.Remove(c);
            }

            foreach (var c in Controls.OfType<PictureBox>())
            {
                Controls.Remove(c);
            }
            foreach (var c in Controls.OfType<Label>())
            {
                c.Text = "";
                c.Visible = false; 
                //Controls.Remove(c);
            }

            buttons = new List<Button>();
            pic = new List<PictureBox>();
            lab = new List<Label>();

            searchThread = new Thread(Searcher);
            searchThread.Start(this);



        }

        private void button3_Click(object sender, EventArgs e)
        {
            SwitchControls(false, Program.form);
        }
    }
}
