using Microsoft.Win32;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Medvedeva_PrivateAdertisementService
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private int userId;
        private int? adId;
        private string selectedImagePath = null;

        public AddEditPage(int userId, int? adId = null)
        {
            InitializeComponent();
            this.userId = userId;
            this.adId = adId;

            LoadComboboxes();

            if (adId != null)
                LoadAdData();
        }

        private void LoadComboboxes()
        {
            using (var db = new Entities())
            {
                CityBox.ItemsSource = db.Cities.ToList();
                CityBox.DisplayMemberPath = "CityName";
                CityBox.SelectedValuePath = "CityID";

                CategoryBox.ItemsSource = db.Categories.ToList();
                CategoryBox.DisplayMemberPath = "CategoryName";
                CategoryBox.SelectedValuePath = "CategoryID";

                TypeBox.ItemsSource = db.AdTypes.ToList();
                TypeBox.DisplayMemberPath = "TypeDescription"; 
                TypeBox.SelectedValuePath = "AdTypeID";

                StatusBox.ItemsSource = db.Statuses.ToList();
                StatusBox.DisplayMemberPath = "StatusName";
                StatusBox.SelectedValuePath = "StatusID";
            }
        }

        private void LoadAdData()
        {
            using (var db = new Entities())
            {
                var ad = db.Ads.FirstOrDefault(a => a.AdID == adId);
                if (ad == null) return;

                TitleBox.Text = ad.Title;
                DescriptionBox.Text = ad.Description;
                PriceBox.Text = ad.Price.ToString();

                CityBox.SelectedValue = ad.CityID;
                CategoryBox.SelectedValue = ad.CategoryID;
                TypeBox.SelectedValue = ad.AdTypeID;
                StatusBox.SelectedValue = ad.StatusID;

                if (!string.IsNullOrEmpty(ad.ImagePath))
                    AdImage.Source = new BitmapImage(new Uri(ad.ImagePath, UriKind.RelativeOrAbsolute));
            }
        }

        private void ChooseImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Изображения|*.jpg;*.png;*.jpeg";

            if (dlg.ShowDialog() == true)
            {
                selectedImagePath = dlg.FileName;
                AdImage.Source = new BitmapImage(new Uri(selectedImagePath, UriKind.Absolute));
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TitleBox.Text) ||
                string.IsNullOrWhiteSpace(PriceBox.Text))
            {
                MessageBox.Show("Заполните обязательные поля.\nНазвание и цена обязательны.",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!decimal.TryParse(PriceBox.Text, out decimal price))
            {
                MessageBox.Show("Цена должна быть числом!", "Ошибка",
                    MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using (var db = new Entities())
                {
                    Ads ad;

                    if (adId == null)
                    {
                        ad = new Ads
                        {
                            UserID = userId,
                            PostDate = DateTime.Now
                        };

                        db.Ads.Add(ad);
                    }
                    else
                    {
                        ad = db.Ads.FirstOrDefault(a => a.AdID == adId);
                    }

                    ad.Title = TitleBox.Text;
                    ad.Description = DescriptionBox.Text;
                    ad.Price = price;

                    ad.CityID = (int)CityBox.SelectedValue;
                    ad.CategoryID = (int)CategoryBox.SelectedValue;
                    ad.AdTypeID = (int)TypeBox.SelectedValue;
                    ad.StatusID = (int)StatusBox.SelectedValue;

                    if (!string.IsNullOrEmpty(selectedImagePath))
                        ad.ImagePath = selectedImagePath;

                    db.SaveChanges();
                }

                MessageBox.Show("Объявление сохранено!",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();

                    if (NavigationService.Content is UserAdsPage userAdsPage)
                    {
                        userAdsPage.LoadUserAds();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка сохранения:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
