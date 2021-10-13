﻿using Project.BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WpfPosApp;
using WpfPosApp.BLL;

namespace WpfPosApp.DAL
{
    class userDAL
    {
        MyConnection db = new MyConnection();
        #region Select Data from Database
        public DataTable Select()
        {

            DataTable dt = new DataTable();
            try
            {
                string sql = "SELECT * FROM Login";

                SqlCommand cmd = new SqlCommand(sql, db.con);

                SqlDataAdapter adapter = new SqlDataAdapter(cmd);

                db.con.Open();

                adapter.Fill(dt);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);

            }
            finally
            {
                db.con.Close();
            }

            return dt;
        }
        #endregion

        #region Insert Data into Database For User Module
        public bool Insert(UserBLL u)
        {
            bool isSuccess = false;

            try
            {
                string sql = "INSERT INTO Login(Name, Surname, UserName, Password, UserType, Gender, Birth_Date, Img, Added_Date, Added_By) VALUES (@Name, @Surname, @UserName, @Password, @UserType, @Gender, @Birth_Date, @Img, @Added_Date, @Added_By)";

                SqlCommand cmd = new SqlCommand(sql, db.con);

                cmd.Parameters.AddWithValue("@Name", u.Name);
                cmd.Parameters.AddWithValue("@Surname", u.Surname);
                cmd.Parameters.AddWithValue("@UserName", u.UserName);
                cmd.Parameters.AddWithValue("@Password", u.Password);
                cmd.Parameters.AddWithValue("@UserType", u.UserType);
                cmd.Parameters.AddWithValue("@Gender", u.Gender);
                cmd.Parameters.AddWithValue("@Birth_Date", u.Birth_Date);
                cmd.Parameters.AddWithValue("@Img", u.Img);
                cmd.Parameters.AddWithValue("@Added_Date", u.Added_Date);
                cmd.Parameters.AddWithValue("@Added_By", u.Added_By);

                db.con.Open();

                int rows = cmd.ExecuteNonQuery();

                if(rows>0)
                {
                    isSuccess = true;
                }
                else
                {
                    isSuccess = false;
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                db.con.Close();
            }

            return isSuccess;
        }
        #endregion

        #region Update Data In DataBase (User Module)
        public bool Update(UserBLL u)
        {
            bool isSuccess = false;

            try
            {
                if (MessageBox.Show("Click YES to save the changes", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "UPDATE Login SET Name=@Name,Surname=@Surname,UserName=@UserName,Password=@Password,UserType=@UserType,Gender=@Gender,Birth_Date=@Birth_Date, Img=@Img,Added_Date=@Added_Date, Added_By=@Added_By Where UserID=@UserID";
                    SqlCommand cmd = new SqlCommand(sql, db.con);

                    cmd.Parameters.AddWithValue("@Name", u.Name);
                    cmd.Parameters.AddWithValue("@Surname", u.Surname);
                    cmd.Parameters.AddWithValue("@UserName", u.UserName);
                    cmd.Parameters.AddWithValue("@Password", u.Password);
                    cmd.Parameters.AddWithValue("@UserType", u.UserType);
                    cmd.Parameters.AddWithValue("@Gender", u.Gender);
                    cmd.Parameters.AddWithValue("@Birth_Date", u.Birth_Date);
                    cmd.Parameters.AddWithValue("@Img", u.Img);
                    cmd.Parameters.AddWithValue("@Added_Date", u.Added_Date);
                    cmd.Parameters.AddWithValue("@Added_By", u.Added_By);
                    cmd.Parameters.AddWithValue("@UserID", u.UserID);

                    db.con.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                db.con.Close();
            }
            return isSuccess;
        }
        #endregion

        #region Delete Data From Database (User Module)

        public bool Delete(UserBLL u)
        {
            bool isSuccess = false;

            try
            {
                if (MessageBox.Show("Are you sure you want to delete this Record?  Click YES to confirm", "CONFIRM", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    string sql = "DELETE FROM Login WHERE UserID = @UserID";

                    SqlCommand cmd = new SqlCommand(sql, db.con);

                    cmd.Parameters.AddWithValue("@UserID", u.UserID);

                    db.con.Open();

                    int rows = cmd.ExecuteNonQuery();

                    if (rows > 0)
                    {
                        isSuccess = true;
                    }
                    else
                    {
                        isSuccess = false;
                    }
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                db.con.Close();
            }

            return isSuccess;
        }

        #endregion

    }
}