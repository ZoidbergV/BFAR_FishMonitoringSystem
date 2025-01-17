﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Project.BLL;
using Project.DAL;
using System.Data.SqlClient;
using System.Data;
using System.IO;
using WpfPosApp.BLL;
using WpfPosApp.DAL;
using Microsoft.Win32;
using FireSharp.Config;
using FireSharp;
using System.Drawing;
using Image = System.Drawing.Image;
using System.Drawing.Imaging;
using Color = System.Windows.Media.Color;
using FirebaseAdmin.Messaging;
using Newtonsoft.Json;
using System.Net.Http;
using Google.Cloud.Firestore;

namespace WpfPosApp
{
    /// <summary>
    /// Interaction logic for frmUser.xaml
    /// </summary>
    public partial class frmUser : UserControl
    {
        MyConnection db = new MyConnection();

        SqlCommand cmd;
        SqlDataAdapter adapt;
        //ID variable used in Updating and Deleting Record 
        int UserID = 0;
        string imgLoc = "user.png";
        string sourcePath = "";
        string destinationPath = "";
         


        // Global Variabel For The Image To Delte
        string rowHeaderImage;


        bool drag = false;
       

        loginBLL uu = new loginBLL();
        loginDAL udal = new loginDAL();

        UserBLL u = new UserBLL();
        userDAL dal = new userDAL();
        frmDealersandCustomers dea;
        frmEmployee emp;
        frmFishDetails p;
        frmDealers deal;

        

        public frmUser()
        {
            InitializeComponent();
            DisplayData();
            showdataGender();
            showdataUserType();
            Colors();
        }

        private void DisplayData()
        {
            db.con.Open();
            DataTable dt = new DataTable();
            adapt = new SqlDataAdapter("SELECT UserID, Name, Surname, Username, Password, UserType [User Type], Gender [Gender], Birth_Date [Birth Date], Img, isActive, Added_Date [Added Time], Added_By [Added By] FROM Login", db.con);
            adapt.Fill(dt);
            grid_User.ItemsSource = dt.DefaultView;
            db.con.Close();
        }

        //Clear Data  
        private void ClearData()
        {
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtUsername.Text = "";
            txtPassword.Text = "";
            cmbUserType.Text = "";
            cmbSex.Text = "";
            dtpBirth.Text = "";

            //UserID = 0;
            string paths = System.Windows.Forms.Application.StartupPath.Substring(0, (System.Windows.Forms.Application.StartupPath.Length - 10));

            string imagePath = paths + "\\Images\\user.png";
            imgBox.ImageSource = new BitmapImage(new Uri(imagePath));
            imgLoc = "user.png";
        }

        public void showdataGender()
        {

            db.con.Open();
            cmd = new SqlCommand("Select GenderID,GenderType from Gender", db.con);
            adapt = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
            db.con.Close();

            DataRow Filaa = dt.NewRow();
            Filaa["GenderType"] = "Select Gender";
            cmbSex.DisplayMemberPath = "GenderType";
            cmbSex.SelectedValuePath = "GenderID";
            cmbSex.ItemsSource = dt.DefaultView;
            db.con.Close();
        }


        public void showdataUserType()
        {
            db.con.Open();
            cmd = new SqlCommand("Select UserTypeID,UserType from UserType", db.con);
            adapt = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            adapt.Fill(dt);
            db.con.Close();

            DataRow Filaa = dt.NewRow();
            Filaa["UserType"] = "Select UserType";
            cmbUserType.DisplayMemberPath = "UserType";
            cmbUserType.SelectedValuePath = "UserTypeID";
            cmbUserType.ItemsSource = dt.DefaultView;
            db.con.Close();
        }

        public void Colors()
        {
            SolidColorBrush bbrush = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            SolidColorBrush wbrush = new SolidColorBrush(Color.FromRgb(255, 255, 255));


            txtAddedBy.Foreground = bbrush;
            txtFirstName.Foreground = bbrush;
            txtLastName.Foreground = bbrush;
            txtPassword.Foreground = bbrush;
            txtUsername.Foreground = bbrush;
            cmbSex.Foreground = wbrush;
            cmbUserType.Foreground = wbrush;
            dtpBirth.Foreground = bbrush;

        }
        private void btnInsert_Click(object sender, RoutedEventArgs e)
        {            


            try {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(cmbUserType.Text) || string.IsNullOrWhiteSpace(dtpBirth.Text)) {

                    MessageBox.Show("Please Complete the Form to Proceed");
                }
                else { 
                cmd = new SqlCommand("Select count(*) from Login where Username= @Username", db.con);
                    cmd.Parameters.AddWithValue("@Username", this.txtUsername.Text);
                    db.con.Open();
                    var result = (int)cmd.ExecuteScalar();
                    db.con.Close();
                    if (result != 0)
                    {
                        MessageBox.Show("Username Already Exist");
                    }
                    else
                    {
                        if (imgLoc != "")
                        {
                            u.Name = txtFirstName.Text;
                            u.Surname = txtLastName.Text;
                            u.UserName = txtUsername.Text;
                            u.Password = txtPassword.Text;
                            u.UserType = cmbUserType.Text;
                            u.Gender = cmbSex.Text;
                            u.Birth_Date = dtpBirth.Text;
                            u.Img = imgLoc;
                            u.Added_Date = DateTime.Now;

                            //Getting username of logged in user
                            string loggedUsr = frmLogin.loggedIn;
                            loginBLL usr = udal.GetIDFromUsername(loggedUsr);

                            u.Added_By = usr.UserID;

                        }
                        bool success = dal.Insert(u);

                        if (success == true)
                        {
                            MessageBox.Show("New User Added Succesfully.");
                            DataTable dt = dal.Select();
                            grid_User.ItemsSource = dt.DefaultView;

                            ClearData();
                            
                        }
                        else
                        {
                            MessageBox.Show("Failed to Add New User");
                        }

                    }

                }
            


            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        

        }   
    
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            
            try {
                if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text) || string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(cmbUserType.Text) || string.IsNullOrWhiteSpace(dtpBirth.Text))
                {

                    MessageBox.Show("Please Complete the Form to Proceed");
                }
                else
                {
                    if (imgLoc != "")
                    {
                        u.UserID = int.Parse(txtUserID.Text);
                        u.Name = txtFirstName.Text;
                        u.Surname = txtLastName.Text;
                        u.UserName = txtUsername.Text;
                        u.Password = txtPassword.Text;
                        u.UserType = cmbUserType.Text;
                        u.Gender = cmbSex.Text;
                        u.Birth_Date = dtpBirth.Text;
                        u.Img = imgLoc;
                        u.Added_Date = DateTime.Now;
                        //Getting username of logged in user
                        string loggedUsr = frmLogin.loggedIn;
                        loginBLL usr = udal.GetIDFromUsername(loggedUsr);

                        u.Added_By = usr.UserID;




                    }

                    bool success = dal.Update(u);

                    if (success == true)
                    {
                        MessageBox.Show("User Updated Successfully.");
                        DataTable dt = dal.Select();
                        grid_User.ItemsSource = dt.DefaultView;

                        ClearData();
                    }
                }

            }

               
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
              
                u.UserID = int.Parse(txtUserID.Text);


                //Remove the Physical file of the User Profile

               
                bool success = dal.Delete(u);

                if (success == true)
                {
                    MessageBox.Show("User Dleted Successfully.");

                    ClearData();
                    DisplayData();
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            ClearData();
            DisplayData();

        }

        private void grid_User_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Console.WriteLine(txtFirstName.Text);
           try
            {
                DataGrid gd = (DataGrid)sender;
                DataRowView row_selected = gd.SelectedItem as DataRowView;
                if (row_selected != null)
                {
                    txtUserID.Text = row_selected[0].ToString();
                    txtFirstName.Text = row_selected[1].ToString();
                    txtLastName.Text = row_selected[2].ToString();
                    txtUsername.Text = row_selected[3].ToString();
                    txtPassword.Text = row_selected[4].ToString();
                    cmbUserType.Text = row_selected[5].ToString();
                    cmbSex.Text = row_selected[6].ToString();
                    dtpBirth.Text = row_selected[7].ToString();
                    imgLoc = row_selected[8].ToString();

                    //Update the Value of Global Variable rowheaderImage
                    rowHeaderImage = imgLoc;

                    string paths = System.Windows.Forms.Application.StartupPath.Substring(0, (System.Windows.Forms.Application.StartupPath.Length - 10));
                    if(imgLoc !="user.png")
                     {
                         string imagePath = paths + "\\Images\\" + imgLoc;
                         imgBox.ImageSource = new BitmapImage(new Uri(imagePath));

                     }
                     else
                     {
                         string imagePath = paths + "\\Images\\user.png";
                         imgBox.ImageSource = new BitmapImage(new Uri(imagePath)); 
                     } 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            } 
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog open = new System.Windows.Forms.OpenFileDialog();

            open.Filter = "Image Files (*.jpg; *.jpeg; *.png; *.PNG; *gifs;)|*.jpg; *.jpeg; *.png; *.PNG; *gifs";

            if(open.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if(open.CheckFileExists)
                {
                    imgBox.ImageSource = new BitmapImage(new Uri(open.FileName));

                    string ext = System.IO.Path.GetExtension(open.FileName);

                    Random random = new Random();
                    int RandInt = random.Next(0, 1000);

                    imgLoc = "POS_USER" + RandInt + ext;

                    sourcePath = open.FileName;

                    string paths = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.Length - 10);

                    destinationPath = paths + "\\Images\\" + imgLoc;

                    File.Copy(sourcePath, destinationPath);


                }
            }

        }

        private void UsrsIcn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            frmUserSettings usr = new frmUserSettings(this);
            gridUser.Children.Add(usr);
           
        }

        private void BtnPrint_Click(object sender, RoutedEventArgs e)
        {
            frmDataReport frm = new frmDataReport(this, dea, emp, p, deal);
            frm.LoadUserReport();
            frm.ShowDialog();
        }


        FirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "Y3V6p5QzcWzXeKXIpBpsBwcQ4mZkisL7zgzpE3Tj",
            BasePath = "https://bfar-testproj-default-rtdb.asia-southeast1.firebasedatabase.app/"
        };

        FirebaseClient client;

        private async void btnUpload_Click(object sender, RoutedEventArgs e)
        {
            dal.ConnecttoFirebase();
            
            
            client = new FirebaseClient(config);
            DataTable dt = dal.Select();

            //Console.WriteLine(dal.DataTableToJSONWithJSONNet(dt));
            try
            {
               foreach (DataRow data in dt.Rows)
                {

                    string paths = System.Windows.Forms.Application.StartupPath.Substring(0, System.Windows.Forms.Application.StartupPath.Length - 10);
                    string imagePath = paths + "\\Images\\" + data["Img"].ToString();

                    Image img = new Bitmap(imagePath);
                    MemoryStream ms = new MemoryStream();
                    img.Save(ms, ImageFormat.Jpeg);

                    byte[] a = ms.GetBuffer();

                    string output = Convert.ToBase64String(a);
                    
                    u.UserID = Convert.ToInt32(data["UserID"]);
                    u.Name = data["Name"].ToString();
                    u.Surname = data["Surname"].ToString();
                    u.UserName = data["UserName"].ToString();
                    u.Password = data["Password"].ToString();
                    u.UserType = data["UserType"].ToString();
                    u.Gender = data["Gender"].ToString();
                    u.Birth_Date = data["Birth_Date"].ToString();
                    u.Img = output;
                    u.Added_Date = DateTime.Now;

                    //Getting username of logged in user
                    string loggedUsr = frmLogin.loggedIn;
                    loginBLL usr = udal.GetIDFromUsername(loggedUsr);

                    u.Added_By = usr.UserID;

                    dal.AddUsertoFirebase(u);

                    await client.SetAsync("Test/" + data["UserID"], u);
                }

                Console.WriteLine("Success");
            }
            catch { } 
        }


        private async void test(object sender, RoutedEventArgs e)
        {
            string to = "";
            FirestoreDb firestoreDatabase;
            string path = AppDomain.CurrentDomain.BaseDirectory + @"bfar-testproj.json";
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", path);

            firestoreDatabase = FirestoreDb.Create("bfar-testproj");
            MessageBox.Show("Connection Success");

            CollectionReference citiesRef = firestoreDatabase.Collection("devices");
            Query query = citiesRef.WhereEqualTo("userId", 1074);
            QuerySnapshot querySnapshot = await query.GetSnapshotAsync();
            foreach (DocumentSnapshot documentSnapshot in querySnapshot.Documents)
            {
                Console.WriteLine("Document {0} returned by queryuserid", documentSnapshot.Id);
                to = documentSnapshot.Id;
            }

            var task = this.NotifyAsync(to, "Testing Message", "Sir ian pasabi kung nareceived mo itong message");
     
            await task;
           


        }

        public async Task<bool> NotifyAsync(string to, string title, string body)
        {
            try
            {
                // Get the server key from FCM console
                var serverKey = string.Format("key={0}", "AAAAunU-lBc:APA91bGgf6ETDyPEUltiC1r4IFvoXz8a20Gf2SxdOvQ-ahkkqkrIHKzd9xJtbkLfRcpEAPcTfaJwTsnIwolCgjjVvGvoRJmuN8VZIqBghKev4O6aGZxVY1zVEF_AvNU5k9DnfIdn_fMA");

                // Get the sender id from FCM console
                var senderId = string.Format("id={0}", "800830952471");

                var data = new
                {
                    to, // Recipient device token
                    notification = new { title, body }
                };

                // Using Newtonsoft.Json
                var jsonBody = JsonConvert.SerializeObject(data);

                using (var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://fcm.googleapis.com/fcm/send"))
                {
                    httpRequest.Headers.TryAddWithoutValidation("Authorization", serverKey);
                    httpRequest.Headers.TryAddWithoutValidation("Sender", senderId);
                    httpRequest.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                    using (var httpClient = new HttpClient())
                    {
                        var result = await httpClient.SendAsync(httpRequest);

                        if (result.IsSuccessStatusCode)
                        {
                            return true;
                        }
                        else
                        {
                            // Use result.StatusCode to handle failure
                            // Your custom error handler here
                            //_logger.LogError($"Error sending notification. Status Code: {result.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
               // _logger.LogError($"Exception thrown in Notify Service: {ex}");
            }

            return false;
        }

    }







    }
