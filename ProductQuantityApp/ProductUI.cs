using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;

namespace ProductQuantityApp
{
    public partial class ProductUI : Form
    {
        public ProductUI()
        {
            InitializeComponent();
        }

        string connectionString = ConfigurationManager.ConnectionStrings["ProductDB"].ConnectionString;

        
        private void saveButton_Click(object sender, EventArgs e)
        {

            string code = codeTextBox.Text;
            string description = descriptionTextBox.Text; ;
            int quantity=int.Parse(quantityTextBox.Text);

            if(IsProductCodeExists(code))
            {
                MessageBox.Show("Product Code Already Exists !");


                SqlConnection connection = new SqlConnection(connectionString);

                int totalquantity=GetQuantity(code)+quantity;

                string query = "UPDATE Product SET Quantity='"+totalquantity+"' WHERE productCode='" + code + "'";
               
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
              int rowAffected=command.ExecuteNonQuery();
                connection.Close();

                if (rowAffected > 0)
                {
                    MessageBox.Show("Quantity is updated successfully!");
                    GetTextBoxesClear();
                }

                else
                {
                    MessageBox.Show("Update Failed !");
                }

               
            }

            else if (codeTextBox.Text.Length < 3)
            {
                MessageBox.Show("You Must Be Enter The Above 3 Characters!");
            }

            else if (quantity < 0)
            {
                MessageBox.Show("Quantity must be greater than 0");
               
            }


            else
            {
                SqlConnection connection = new SqlConnection(connectionString);

                string query = "INSERT INTO Product Values('" + code + "','" + description + "','" + quantity + "')";

                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                int rowAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowAffected > 0)
                {
                    MessageBox.Show("Product is inserted successfully!");
                    GetTextBoxesClear();
                }

                else
                {
                    MessageBox.Show("Insertion Failed !");
                }

               
            }
           

        }


        private void GetTextBoxesClear() 
        {


            codeTextBox.Clear();
            descriptionTextBox.Clear();
            quantityTextBox.Clear();
        }


        public bool IsProductCodeExists(string code)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT productCode FROM Product WHERE productCode='" + code + "'";
            bool isProductCodeExists = false;

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                isProductCodeExists = true;
                break;
            }
            reader.Close();
            connection.Close();
            return isProductCodeExists;
        }


        private int GetQuantity(string code)
        {
            int quantity = 0;
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT Quantity FROM Product WHERE productCode='" + code + "'";
   

            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                int sdata = int.Parse(reader["Quantity"].ToString());
                quantity += sdata;
            }
            reader.Close();
            connection.Close();
            return quantity;
        
        }

        private void showButton_Click(object sender, EventArgs e)
        {
            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT * FROM Product ";


            SqlCommand command = new SqlCommand(query, connection);
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            List<Product> productList = new List<Product>();
           

            while (reader.Read())
            {
                Product products = new Product();
                products.id = int.Parse(reader["productId"].ToString());
                products.code=reader["productCode"].ToString();
                products.description=reader["Description"].ToString();
                products.quantity = int.Parse(reader["Quantity"].ToString());

                productList.Add(products);
            }
            reader.Close();
            connection.Close();
            LoadProductListView(productList);


            totalTextBox.Text=GetTotalQuantity().ToString();
            
             
            }

        public void LoadProductListView(List<Product>product)

        {
            productListView.Items.Clear();
            foreach (var Product in product)
            {
                ListViewItem item = new ListViewItem(Product.id.ToString());
                item.SubItems.Add(Product.code);
                item.SubItems.Add(Product.description);
                item.SubItems.Add(Product.quantity.ToString());

                productListView.Items.Add(item);
            }
            
        
        }

        public int GetTotalQuantity()
        {

            SqlConnection connection = new SqlConnection(connectionString);

            string query = "SELECT Sum(Quantity) FROM Product ";


            SqlCommand command = new SqlCommand(query, connection);
              connection.Open();
              int totalQuantity = (int)command.ExecuteScalar();

            
             connection.Close();
             return totalQuantity;
            
        }
    }
}
