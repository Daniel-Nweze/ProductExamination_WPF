using Newtonsoft.Json;
using Shared.Models;
using Shared.Services;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;



namespace MainApp
{
    public partial class MainWindow : Window
    {
        private readonly ProductService _productService;
        private ObservableCollection<Product> _productList = [];
        private Product? _selectedProduct;

        public MainWindow(ProductService productService)
        {
            InitializeComponent();
            _productService = productService;
            UpdateListView();
            RefreshFocus();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            HandleSave();
        }

        private void Save_KeyUp(object sender, KeyEventArgs e)
        {
            if (Key.Enter == e.Key)
                BtnSave_Click(sender, e);
        }

        private void Btn_Delete_Click(object sender, RoutedEventArgs e)
        {
            DeleteProduct(sender);
        }

        #region CRUD
        private void HandleSave()
        {
            if (HandleErrors())
                return;

            if (_selectedProduct == null)
            {
                AddProduct();
            }
            else
            {
                UpdateProduct();
            }
            RefreshFocus();
        }

        private void AddProduct()
        {
            if (HandleErrors())
                return;

            Product product = new Product();

            if (decimal.TryParse(ProductPrice.Text, out decimal price))
            {
                product.Name = ProductName.Text;
                product.Price = price;
                product.ProductCategory.CategoryName = CategoryName.Text;
            }
            _productService.AddToList(product);

            RefreshFields();
            UpdateListView();
            RefreshFocus();
        }

        private void UpdateProduct()
        {
            if (decimal.TryParse(ProductPrice.Text, out decimal price) && _selectedProduct != null)
            {
                _selectedProduct.Name = ProductName.Text;
                _selectedProduct.Price = price;
                _selectedProduct.ProductCategory.CategoryName = CategoryName.Text;

                _productService.UpdateProduct(_selectedProduct, _selectedProduct.Name, _selectedProduct.Price, _selectedProduct.ProductCategory.CategoryName);
                listViewProducts.Items.Refresh();
                _selectedProduct = null;
                RefreshFields();
            }

        }

        private void DeleteProduct(object sender)
        {
            Button button = (Button)sender;
            if (button.DataContext is Product product)
            {
                _productService.DeleteProduct(product);
                _productList.Remove(product);

                RefreshFields();
                _selectedProduct = null;
                RefreshFocus();
            }
        }

        #endregion

        #region Window Event Handlers      
        private bool HandleErrors()
        {
            if (string.IsNullOrEmpty(ProductName.Text) || string.IsNullOrEmpty(CategoryName.Text) || string.IsNullOrEmpty(ProductPrice.Text))
            {
                MessageBoxError("Du måste fylla i alla fält.", "Error");
                return true;
            }

            if (!decimal.TryParse(ProductPrice.Text, out decimal price) || price <= 0)
            {
                MessageBoxError("Angivet pris måste överstiga 0.", "Error");
                return true;
            }

            if (ProductName.Text == CategoryName.Text)
            {
                MessageBoxError("Kategori- och produktnamn får inte sparas med likadana värden.", "Error");
                return true;
            }

            return false;
        }

        private void MessageBoxError(string errorText, string errorCaption)
        {
            MessageBox.Show(errorText, errorCaption, MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void UpdateListView()
        {
            _productList = new ObservableCollection<Product>(_productService.GetAllProducts());
            listViewProducts.ItemsSource = _productList;
        }

        private void RefreshFields()
        {
            ProductName.Text = "";
            ProductPrice.Text = "";
            CategoryName.Text = "";
        }

        private void RefreshFocus()
        {
            CategoryName.Focus();
            CategoryName.SelectionStart = CategoryName.Text.Length;
        }

        private void listViewProducts_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (listViewProducts.SelectedItem is Product selectedProduct)
            {
                _selectedProduct = selectedProduct;

                ProductName.Text = _selectedProduct.Name;
                ProductPrice.Text = _selectedProduct.Price.ToString();
                CategoryName.Text = _selectedProduct.ProductCategory.CategoryName;
                RefreshFocus();
            }

        }

        #endregion

    }
}