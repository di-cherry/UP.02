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
    /// Логика взаимодействия для AdsPage.xaml
    /// </summary>
    public partial class AdsPage : Page
    {
        private int currentUserId;

        public AdsPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadAds();
        }

        private void LoadAds()
        {
            try
            {
                using (var db = new Entities())
                {
                    var ads = db.Ads
                        .Select(a => new
                        {
                            a.AdID,
                            a.Title,
                            a.Description,
                            a.Price,
                            a.StatusID,

                            CityName = a.Cities.CityName,
                            CategoryName = a.Categories.CategoryName,
                            AdTypeName = a.AdTypes.TypeDescription,

                            IsOwner = (a.UserID == currentUserId),

                            ImagePath = string.IsNullOrEmpty(a.ImagePath)
                                ? "/Resources/images.jpg"
                                : a.ImagePath
                        })
                        .ToList();
                    CityFilter.ItemsSource = db.Cities.ToList();
                    CityFilter.DisplayMemberPath = "CityName";
                    CityFilter.SelectedValuePath = "CityID";

                    CategoryFilter.ItemsSource = db.Categories.ToList();
                    CategoryFilter.DisplayMemberPath = "CategoryName";
                    CategoryFilter.SelectedValuePath = "CategoryID";

                    TypeFilter.ItemsSource = db.AdTypes.ToList();
                    TypeFilter.DisplayMemberPath = "TypeDescription";
                    TypeFilter.SelectedValuePath = "AdTypeID";

                    AdsList.ItemsSource = ads;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка загрузки объявлений:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddAd_Click(object sender, RoutedEventArgs e)
        {
            (Application.Current.MainWindow as MainWindow)
                ?.MainFrame.Navigate(new AddEditPage(currentUserId));
        }

        private void EditAd_Click(object sender, RoutedEventArgs e)
        {
            int adId = Convert.ToInt32((sender as Button).Tag);

            (Application.Current.MainWindow as MainWindow)
                ?.MainFrame.Navigate(new AddEditPage(currentUserId, adId));
        }

        private void DeleteAd_Click(object sender, RoutedEventArgs e)
        {
            int adId = Convert.ToInt32((sender as Button).Tag);

            if (MessageBox.Show("Удалить объявление?",
                "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Warning)
                != MessageBoxResult.Yes) return;

            try
            {
                using (var db = new Entities())
                {
                    var ad = db.Ads.FirstOrDefault(a => a.AdID == adId);
                    if (ad == null) return;

                    db.Ads.Remove(ad);
                    db.SaveChanges();
                }

                MessageBox.Show("Объявление удалено.",
                    "Информация", MessageBoxButton.OK, MessageBoxImage.Information);

                LoadAds();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка удаления:\n" + ex.Message,
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ResetFilters_Click(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = "";
            CityFilter.SelectedIndex = -1;
            CategoryFilter.SelectedIndex = -1;
            TypeFilter.SelectedIndex = -1;
        }

        private void FilterChanged(object sender, EventArgs e)
        {
            UpdateAds();
        }

        private void UpdateAds()
        {
            if (!IsInitialized) return;

            try
            {
                using (var db = new Entities())
                {
                    var ads = db.Ads
                        .Where(a => a.StatusID == 1)
                        .Select(a => new
                        {
                            a.AdID,
                            a.Title,
                            a.Description,
                            a.Price,

                            CityID = a.CityID,
                            CategoryID = a.CategoryID,
                            TypeID = a.AdTypeID,

                            CityName = a.Cities.CityName,
                            CategoryName = a.Categories.CategoryName,
                            AdTypeName = a.AdTypes.TypeDescription,
                            IsOwner = (a.UserID == currentUserId),

                            ImagePath = string.IsNullOrEmpty(a.ImagePath)
                                ? "/Resources/images.jpg"
                                : a.ImagePath
                        })
                        .ToList();

                    if (!string.IsNullOrWhiteSpace(SearchBox.Text))
                    {
                        string search = SearchBox.Text.ToLower();

                        ads = ads.Where(a =>
                            (a.Title != null && a.Title.ToLower().Contains(search)) ||
                            (a.Description != null && a.Description.ToLower().Contains(search))
                        ).ToList();
                    }

                    if (CityFilter.SelectedItem != null)
                    {
                        int selectedCity = (int)CityFilter.SelectedValue;
                        ads = ads.Where(a => a.CityID == selectedCity).ToList();
                    }

                    if (CategoryFilter.SelectedItem != null)
                    {
                        int selectedCategory = (int)CategoryFilter.SelectedValue;
                        ads = ads.Where(a => a.CategoryID == selectedCategory).ToList();
                    }

                    if (TypeFilter.SelectedItem != null)
                    {
                        int selectedType = (int)TypeFilter.SelectedValue;
                        ads = ads.Where(a => a.TypeID == selectedType).ToList();
                    }

                    AdsList.ItemsSource = ads;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при фильтрации: {ex.Message}");
            }
        }
    }
}