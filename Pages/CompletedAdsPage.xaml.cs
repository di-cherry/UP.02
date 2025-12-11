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

namespace Medvedeva_PrivateAdertisementService.Pages
{
    /// <summary>
    /// Логика взаимодействия для CompletedAdsPage.xaml
    /// </summary>
    public partial class CompletedAdsPage : Page
    {
        private int currentUserId;

        public CompletedAdsPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            LoadCompletedAds();
            LoadTotalProfit();
        }

        public void LoadCompletedAds()
        {
            try
            {
                using (var db = new Entities())
                {
                    var ads = db.Ads
                        .Where(a => a.UserID == currentUserId && a.StatusID == 2) // Статус "Завершено"
                        .Select(a => new
                        {
                            a.AdID,
                            a.Title,
                            a.Description,
                            Price = a.Price,
                            PostDate = a.PostDate,
                            CityName = a.Cities.CityName,
                            CategoryName = a.Categories.CategoryName,
                            AdTypeName = a.AdTypes.TypeDescription,
                            ImagePath = string.IsNullOrEmpty(a.ImagePath)
                                        ? "/Resources/images.jpg"
                                        : a.ImagePath
                        })
                        .ToList();

                    CompletedAdsList.ItemsSource = ads;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки завершенных объявлений: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void LoadTotalProfit()
        {
            try
            {
                using (var db = new Entities())
                {
                    var user = db.Users.FirstOrDefault(u => u.UserID == currentUserId);
                    if (user != null)
                    {
                        TotalProfitText.Text = $"Общий доход: {user.TotalProfit} ₽";
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки дохода: {ex.Message}",
                    "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}