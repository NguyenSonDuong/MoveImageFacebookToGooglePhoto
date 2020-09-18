using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xNet;

namespace FacebookMessage
{
    public partial class Form1 : Form
    {
        private String cookie = "";
        private bool isAll = false;
        public Form1()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Thread thread = new Thread(() =>
            {
                String cookie = "";
                if (File.Exists("cookie.init"))
                {
                    cookie = File.ReadAllText("cookie.init");
                }
                if (String.IsNullOrEmpty(cookie))
                {
                    SetCookie setcookie = new SetCookie((text) =>
                    {
                        try
                        {
                            File.WriteAllText("cookie.init", text);
                        }
                        catch (Exception ex)
                        {

                        }
                        cookie = text;
                    });
                    setcookie.ShowDialog();
                }
                getAccessToken(cookie);
            });
            thread.IsBackground = true;
            thread.Start();
        }
        // Lấy token facebook
        public void getAccessToken(String cookie)
        {
            if (string.IsNullOrEmpty(cookie))
            {
                MessageBox.Show("Vui lòng nhập cookie");
                return;
            }
            String user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36";
            try
            {
                HttpRequest http = getRequest(cookie, user_agent);
                richTextBox1.Invoke(new MethodInvoker(() =>
                {
                    richTextBox1.AppendText("Đang trong quá trình lấy token");
                }));
                String dataAccessToken = http.Get(ModerConst.URL_GETCOOKIE).ToString();

                if (dataAccessToken.Contains("EAAA"))
                {
                    String endString = "\\\",\\\"useLocalFilePreview\\\"";
                    int start = dataAccessToken.IndexOf("EAAA");
                    int end = dataAccessToken.IndexOf(endString);
                    String t = dataAccessToken.Substring(start, end - start);
                    textBox2.Invoke(new MethodInvoker(() =>
                    {
                        textBox2.Text = t;
                    }));
                    
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nĐã lấy thành công token");
                    }));
                    File.WriteAllText("tookie.init", textBox2.Text);
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nĐã lưu thành công token");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Update tooken facebook is successful\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                }
                else
                {
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nĐã gặp lỗi");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Update tooken facebook is ERROR\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    MessageBox.Show("Lỗi Request vui lòng kiểm tra lại Cookie và UserAgent");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                File.AppendAllText("log.lg", String.Format("Status: Update tooken facebook is ERROR: {0}\tTime: {1} {2}\n", ex.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
            }

        }

        HttpRequest getRequest(String cookie, String user_agent)
        {
            HttpRequest http = new HttpRequest();
            http.Cookies = new CookieDictionary();
            if (!string.IsNullOrEmpty(user_agent))
                http.UserAgent = user_agent;
            if (!string.IsNullOrEmpty(cookie))
            {
                AddCookie(http, cookie);
            }
            return http;
        }

        void AddCookie(HttpRequest http, string cookie)
        {
            var temp = cookie.Split(';');
            foreach (var item in temp)
            {
                var temp2 = item.Split('=');
                if (temp2.Count() > 1)
                {
                    http.Cookies.Add(temp2[0], temp2[1]);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            File.AppendAllText("log.lg", String.Format("Status: Open application\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
            if (File.Exists("tookie.init"))
            {
                String token = File.ReadAllText("tookie.init");
                if (!String.IsNullOrEmpty(token))
                {
                    textBox2.Text = token;
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("Nạp token thành công");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Update tooken facebook is successful\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                }
            }
            if (File.Exists("tokenGoogle.init"))
            {
                String token = File.ReadAllText("tokenGoogle.init");
                if (!String.IsNullOrEmpty(token))
                {
                    textBox3.Text = token;
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nNạp token Google thành công");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Update tooken facebook is successful\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                }
            }
        }
        private void button4_Click(object sender, EventArgs e)
        {
            File.WriteAllText("tokenGoogle.init", textBox3.Text);
            richTextBox1.Invoke(new MethodInvoker(() =>
            {
                richTextBox1.AppendText("\nNạp token Google thành công");
            }));
        }
        private void button5_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                Thread t = new Thread(() =>
                {
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nBắt đầu quá trình lấy thông tin ảnh");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Start get ID image of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    List<String> idL = getListIDImage(textBox4.Text);
                    #region
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKết thúc quá trình lấy thông tin ảnh\n===========================");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  End get infor ID of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nBắt đầu quá trình tải ảnh");
                    }));
                    #endregion
                    File.AppendAllText("log.lg", String.Format("Status: Start download image: \tTime: {1} {2}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    if (idL.Count <= 0)
                    {
                        DownloadImage(folder.SelectedPath + "\\", textBox4.Text);

                    }
                    else
                    {
                        foreach (String id in idL)
                        {
                            DownloadImage(folder.SelectedPath + "\\", id);
                        }
                    }

                    File.AppendAllText("log.lg", String.Format("Status: End download image: \tTime: {1} {2}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKết thúc tải ảnh");
                    }));
                });
                t.IsBackground = true;
                t.Start();
            }


        }

        public List<String> getListIDImage(String id)
        {
            String url = getURL(textBox2.Text, id + "?fields=attachments{subattachments.limit(1000)}");
            List<String> list = new List<string>();
            try
            {
                HttpRequest http = getRequest("", "");
                String outJson = http.Get(url).ToString();
                ListIDImage.Rootobject listID = JsonConvert.DeserializeObject<ListIDImage.Rootobject>(outJson);
                foreach (ListIDImage.Datum1 item in listID.attachments.data[0].subattachments.data)
                {
                    list.Add(item.target.id);
                }
            }
            catch (Exception ex)
            {
                try
                {
                    url = getURL(textBox2.Text, id + "?fields=attachments");
                    HttpRequest http = getRequest("", "");
                    String outJson = http.Get(url).ToString();
                    ImageOnePicture.Rootobject listID = JsonConvert.DeserializeObject<ImageOnePicture.Rootobject>(outJson);
                    foreach (ImageOnePicture.Datum item in listID.attachments.data)
                    {
                        list.Add(item.target.id);
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error: " + e.Message);
                }
            }


            return list;
        }
        public void getListIDAllbum(String id, String url, List<String> list)
        {
            try
            {
                HttpRequest http = getRequest("", "");
                String outJson = http.Get(url).ToString();
                ImageOfAllbum.Rootobject listID = JsonConvert.DeserializeObject<ImageOfAllbum.Rootobject>(outJson);
                foreach (ImageOfAllbum.Datum item in listID.data)
                {
                    list.Add(item.id);
                }
                if (listID.paging != null)
                    if (!String.IsNullOrEmpty(listID.paging.next))
                    {
                        getListIDAllbum(id, listID.paging.next, list);
                    }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }

        }
        public ImageID.Image[] getUrlFromID(String id)
        {
            ImageID.Image[] list = null;
            try
            {
                String url = getURL(textBox2.Text, id + "?fields=images");
                HttpRequest http = getRequest("", "");
                String outJson = http.Get(url).ToString();
                ImageID.Rootobject listIMG = JsonConvert.DeserializeObject<ImageID.Rootobject>(outJson);
                list = listIMG.images;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return list;
        }
        public void DownloadImage(String path, String id)
        {
            String url = getURL(textBox2.Text, id + "?fields=images");
            try
            {
                HttpRequest http = getRequest("", "");
                String outJson = http.Get(url).ToString();
                ImageID.Rootobject list = JsonConvert.DeserializeObject<ImageID.Rootobject>(outJson);
                String pathout = path + "\\" + id;
                if (isAll)
                {
                    Directory.CreateDirectory(pathout);
                    foreach (ImageID.Image item in list.images)
                    {
                        try
                        {
                            http.Get(item.source).ToFile(pathout + "\\h" + item.height + "_w" + item.width + ".jpg");
                        }
                        catch (Exception ex)
                        {
                            richTextBox1.Invoke(new MethodInvoker(() =>
                            {
                                richTextBox1.AppendText("\nLỗi: " + ex.Message);
                            }));
                        }
                    }
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nĐã tải xong ảnh ID: " + id);
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  Download successful image: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                }
                else
                {
                    try
                    {
                        http.Get(list.images[0].source).ToFile(path + "\\" + id + ".jpg");
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nĐã tải xong ảnh ID: " + id);
                        }));
                        File.AppendAllText("log.lg", String.Format("Status:  Download successful image: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    }
                    catch (Exception ex)
                    {
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nLỗi:" + ex.Message);
                        }));
                        File.AppendAllText("log.lg", String.Format("Status:  Download ERROR image: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    }
                }
            }
            catch (Exception ex)
            {
                richTextBox1.Invoke(new MethodInvoker(() =>
                {
                    richTextBox1.AppendText("\nLỗi:" + ex.Message);
                }));
                File.AppendAllText("log.lg", String.Format("Status:  Download ERROR image: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
            }
        }
        public String uploadImage(String link, String token)
        {

            String user_agent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/84.0.4147.135 Safari/537.36";
            HttpRequest http = getRequest("", user_agent);
            http.AddHeader("Authorization", "Bearer " + token);
            http.AddHeader("X-Goog-Upload-Content-Type", "image/png");
            http.AddHeader("X-Goog-Upload-Protocol", "raw");
            byte[] dataa = new HttpRequest().Get(link).ToBytes();
            String httpOut = http.Post("https://photoslibrary.googleapis.com/v1/uploads", dataa, "application/octet-stream").ToString();
            return httpOut;
        }
        public static String getURL(String token, String para)
        {
            return "https://graph.facebook.com/v8.0/" + para + "&access_token=" + token;
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            isAll = checkBox1.Checked;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    String token = textBox3.Text;
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nBắt đầu lấy thông tin bài viết");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: Start get ID image of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    List<String> idL = getListIDImage(textBox4.Text);
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKết thúc lấy thông tin bài viết");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  End get infor ID of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    List<ImageID.Image> list = new List<ImageID.Image>();
                    UpdataToPhotos.Rootobject root = new UpdataToPhotos.Rootobject();
                    root.newMediaItems = new UpdataToPhotos.Newmediaitem[100];
                    int i = 0;
                    File.AppendAllText("log.lg", String.Format("Status:  Start upload image to Google\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nBắt đầu tải ảnh lên Google");
                    }));
                    if (idL.Count <= 0)
                    {
                        String id = textBox4.Text;
                        ImageID.Image[] image = getUrlFromID(id);
                        File.AppendAllText("log.lg", String.Format("Status:  Upload image to Google ID: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        String tokenImg = uploadImage(image[0].source, token);
                        UpdataToPhotos.Newmediaitem newItem = new UpdataToPhotos.Newmediaitem();
                        newItem.simpleMediaItem = new UpdataToPhotos.Simplemediaitem();
                        newItem.simpleMediaItem.fileName = id + ".jpg";
                        newItem.simpleMediaItem.uploadToken = tokenImg;
                        newItem.description = "Đây là ảnh của bài việt ID: " + id;
                        richTextBox1.Invoke(new MethodInvoker(() =>
                            {
                                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                                richTextBox1.AppendText("\nXong ảnh: " + id);

                            }));

                        root.newMediaItems[i] = newItem;
                    }
                    else
                    {
                        foreach (String id in idL)
                        {
                            ImageID.Image[] image = getUrlFromID(id);
                            String tokenImg = uploadImage(image[0].source, token);
                            File.AppendAllText("log.lg", String.Format("Status:  Upload image to Google ID: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                            UpdataToPhotos.Newmediaitem newItem = new UpdataToPhotos.Newmediaitem();
                            newItem.simpleMediaItem = new UpdataToPhotos.Simplemediaitem();
                            newItem.simpleMediaItem.fileName = id + ".jpg";
                            newItem.simpleMediaItem.uploadToken = tokenImg;
                            newItem.description = "Đây là ảnh của bài việt ID: " + id;
                            root.newMediaItems[i] = newItem;
                            richTextBox1.Invoke(new MethodInvoker(() =>
                            {
                                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                                richTextBox1.AppendText("\nXong ảnh: " + id);
                            }));
                            i++;

                        }
                    }
                    File.AppendAllText("log.lg", String.Format("Status:  End upload image to google\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                        richTextBox1.AppendText("\nKết thúc tải ảnh lên Google");
                    }));
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nTạo ảnh trên Google Photos");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  Push to Google Photos\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    HttpRequest http = getRequest("", "");
                    http.AddHeader("Authorization", "Bearer " + token);
                    String dataOut = http.Post("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate", JsonConvert.SerializeObject(root), "application/json").ToString();
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKẾT THÚC");
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  END ALL\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));

                }
                catch (Exception ex)
                {
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nLỗi: " + ex.Message);
                    }));
                    File.AppendAllText("log.lg", String.Format("Status:  Error: {0}\tTime: {1} {2}\n", ex.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));

                }
            });
            th.IsBackground = true;
            th.Start();

        }

        private void textBox4_TextChanged(object sender, EventArgs e)
        {

        }
        private void label6_Click(object sender, EventArgs e)
        {

        }
        private void label5_Click(object sender, EventArgs e)
        {

        }
        private void button2_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folder = new FolderBrowserDialog();
            if (folder.ShowDialog() == DialogResult.OK)
            {
                Thread t = new Thread(() =>
                {
                    String[] liID = textBox1.Text.Split('|');
                    foreach (String idItem in liID)
                    {
                        File.AppendAllText("log.lg", String.Format("Status: Start get ID image of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nBắt đầu quá trình lấy thông tin ảnh: " + idItem);
                        }));

                        List<String> idL = getListIDImage(idItem);
                        #region
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nKết thúc quá trình lấy thông tin ảnh\n===========================");
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: End get ID image of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        File.AppendAllText("log.lg", String.Format("Status: Start download image\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nBắt đầu quá trình tải ảnh: " + idItem);
                        }));
                        #endregion

                        foreach (String id in idL)
                        {
                            DownloadImage(folder.SelectedPath, id);
                        }

                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nKết thúc tải ảnh");
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: End download image\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        Thread.Sleep(new Random().Next(3000, 5000));
                    }

                });
                t.IsBackground = true;
                t.Start();
            }

        }
        private void button1_Click(object sender, EventArgs e)
        {
            Thread th = new Thread(() =>
            {

                try
                {
                    String[] idLi = textBox1.Text.Split('|');
                    foreach (String idItem in idLi)
                    {
                        String token = textBox3.Text;
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nBắt đầu lấy thông tin bài viết: " + idItem);
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: Start get ID image of facebook post: ID {0}\tTime: {1} {2}\n", idItem, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        List<String> idL = getListIDImage(idItem);
                        List<ImageID.Image> list = new List<ImageID.Image>();
                        UpdataToPhotos.Rootobject root = new UpdataToPhotos.Rootobject();
                        root.newMediaItems = new UpdataToPhotos.Newmediaitem[100];
                        int i = 0;
                        File.AppendAllText("log.lg", String.Format("Status: End get ID image of facebook post\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        File.AppendAllText("log.lg", String.Format("Status: Start upload image to Google\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nBắt đầu tải ảnh lên Google");
                        }));
                        foreach (String id in idL)
                        {
                            ImageID.Image[] image = getUrlFromID(id);
                            String tokenImg = uploadImage(image[0].source, token);
                            UpdataToPhotos.Newmediaitem newItem = new UpdataToPhotos.Newmediaitem();
                            newItem.simpleMediaItem = new UpdataToPhotos.Simplemediaitem();
                            newItem.simpleMediaItem.fileName = id + ".jpg";
                            newItem.simpleMediaItem.uploadToken = tokenImg;
                            newItem.description = "Đây là ảnh của bài việt ID: " + id;
                            root.newMediaItems[i] = newItem;
                            richTextBox1.Invoke(new MethodInvoker(() =>
                            {
                                richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                                richTextBox1.AppendText("\nXong ảnh: " + id);
                            }));
                            i++;
                        }
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Regular);
                            richTextBox1.AppendText("\nKết thúc tải ảnh lên Google");
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: End upload image to Google\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nTạo ảnh trên Google Photos");
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: Push image to Google Photos\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        HttpRequest http = getRequest("", "");
                        http.AddHeader("Authorization", "Bearer " + token);
                        String dataOut = http.Post("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate", JsonConvert.SerializeObject(root), "application/json").ToString();
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nXong: " + idItem);
                        }));
                        File.AppendAllText("log.lg", String.Format("Status: End: {0}\tTime: {1} {2}\n", idItem, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        Thread.Sleep(new Random().Next(3000, 5000));
                    }
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKẾT THÚC");
                    }));

                }
                catch (Exception ex)
                {
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nLỗi: " + ex.Message);
                    }));
                    File.AppendAllText("log.lg", String.Format("Status: End: Error: {0}\tTime: {1} {2}\n", ex.Message, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));

                }
            });
            th.IsBackground = true;
            th.Start();
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            new LogView().ShowDialog();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            new WebViewLogin().ShowDialog();

        }

        private void button8_Click(object sender, EventArgs e)
        {
            Thread t = new Thread(() =>
            {
                String token = textBox4.Text;
                String url = getURL(textBox2.Text, textBox4.Text + "/photos?limit=100");
                richTextBox1.Invoke(new MethodInvoker(() =>
                {
                    richTextBox1.AppendText("Đang lấy danh sách ID");
                }));
                List<String> list2 = new List<string>();
                getListIDAllbum(token, url, list2);
                richTextBox1.Invoke(new MethodInvoker(() =>
                {
                    richTextBox1.AppendText("Đã lấy xong danh sách id, Đang lấy danh sách image");
                }));
                List<ImageID.Image> list = new List<ImageID.Image>();
                UpdataToPhotos.Rootobject root = new UpdataToPhotos.Rootobject();
                List<UpdataToPhotos.Newmediaitem> listItem = new List<UpdataToPhotos.Newmediaitem>();
                int i = 0;
                int j = 0;
                foreach (String id in list2)
                {
                    ImageID.Image[] image = getUrlFromID(id);
                    String tokenImg = "";
                    try
                    {
                         tokenImg = uploadImage(image[0].source, textBox3.Text);
                    }
                    catch(Exception ex)
                    {
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                            richTextBox1.SelectionColor = Color.Red;
                            richTextBox1.AppendText("\nLỗi ảnh: " + id);
                            richTextBox1.SelectionColor = Color.Black;
                        }));
                        File.AppendAllText("log.lg", String.Format("Error:  Upload image to Google ID: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        break;
                    }
                    
                    File.AppendAllText("log.lg", String.Format("Status:  Upload image to Google ID: {0}\tTime: {1} {2}\n", id, DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    UpdataToPhotos.Newmediaitem newItem = new UpdataToPhotos.Newmediaitem();
                    newItem.simpleMediaItem = new UpdataToPhotos.Simplemediaitem();
                    newItem.simpleMediaItem.fileName = id + ".jpg";
                    newItem.simpleMediaItem.uploadToken = tokenImg;
                    newItem.description = "Đây là ảnh của Allbum ID: " + id;
                    listItem.Add(newItem);
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.SelectionFont = new Font(richTextBox1.Font, FontStyle.Bold);
                        richTextBox1.AppendText("\nXong ảnh: " + id);
                    }));
                    i++;
                    j++;
                    if (j >= 20)
                    {
                        root.newMediaItems = listItem.ToArray();
                        HttpRequest http2 = getRequest("", "");
                        http2.AddHeader("Authorization", "Bearer " + textBox3.Text);
                        String post = JsonConvert.SerializeObject(root);
                        String dataOut2 = http2.Post("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate", post, "application/json").ToString();
                        richTextBox1.Invoke(new MethodInvoker(() =>
                        {
                            richTextBox1.AppendText("\nKẾT THÚC 100 ẢNH. TIẾP TỤC 100 ẢNH");
                        }));
                        File.AppendAllText("log.lg", String.Format("Status:  END Part\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                        root = new UpdataToPhotos.Rootobject();
                        j = 0;
                    }
                }
                root.newMediaItems = listItem.ToArray();
                File.AppendAllText("log.lg", String.Format("Status:  Push to Google Photos\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                HttpRequest http = getRequest("", "");
                http.AddHeader("Authorization", "Bearer " + token);
                try
                {
                    String dataOut = http.Post("https://photoslibrary.googleapis.com/v1/mediaItems:batchCreate", JsonConvert.SerializeObject(root), "application/json").ToString();
                }
                catch(Exception ex)
                {
                    richTextBox1.Invoke(new MethodInvoker(() =>
                    {
                        richTextBox1.AppendText("\nKẾT THÚC: Error");
                    }));
                    File.AppendAllText("log.lg", String.Format("Error:  END ALL\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));
                    return;
                }
                
                richTextBox1.Invoke(new MethodInvoker(() =>
                {
                    richTextBox1.AppendText("\nKẾT THÚC");
                }));
                File.AppendAllText("log.lg", String.Format("Status:  END ALL\tTime: {0} {1}\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToLongTimeString()));

            });
            t.IsBackground = true;
            t.Start();

        }
    }
}

public class ListIDImage
{

    public class Rootobject
    {
        public string id { get; set; }
        public Attachments attachments { get; set; }
    }

    public class Attachments
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public Subattachments subattachments { get; set; }
    }

    public class Subattachments
    {
        public Datum1[] data { get; set; }
    }

    public class Datum1
    {
        public Media media { get; set; }
        public Target target { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Media
    {
        public Image image { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string src { get; set; }
        public int width { get; set; }
    }

    public class Target
    {
        public string id { get; set; }
        public string url { get; set; }
    }

}
public class ImageID
{

    public class Rootobject
    {
        public Image[] images { get; set; }
        public string id { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string source { get; set; }
        public int width { get; set; }
    }

}
public class ImageOnePicture
{

    public class Rootobject
    {
        public string id { get; set; }
        public Attachments attachments { get; set; }
    }

    public class Attachments
    {
        public Datum[] data { get; set; }
    }

    public class Datum
    {
        public string description { get; set; }
        public Media media { get; set; }
        public Target target { get; set; }
        public string type { get; set; }
        public string url { get; set; }
    }

    public class Media
    {
        public Image image { get; set; }
    }

    public class Image
    {
        public int height { get; set; }
        public string src { get; set; }
        public int width { get; set; }
    }

    public class Target
    {
        public string id { get; set; }
        public string url { get; set; }
    }

}
public class UpdataToPhotos
{

    public class Rootobject
    {
        public Newmediaitem[] newMediaItems { get; set; }
    }

    public class Newmediaitem
    {
        public string description { get; set; }
        public Simplemediaitem simpleMediaItem { get; set; }
    }

    public class Simplemediaitem
    {
        public string fileName { get; set; }
        public string uploadToken { get; set; }
    }

}
public class ImageOfAllbum
{

    public class Rootobject
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
        //public string previous { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class Datum
    {
        public DateTime created_time { get; set; }
        public string id { get; set; }
    }

}