using BigDataOrdersDashboard.Context;
using Microsoft.AspNetCore.Mvc;

namespace BigDataOrdersDashboard.ViewComponents.DashboardViewComponents
{
    public class _DashboardKpiComponentPartial : ViewComponent
    {
        private readonly BigDataDbContext _context;
        public _DashboardKpiComponentPartial(BigDataDbContext context)
        {
            _context = context;
        }

        public IViewComponentResult Invoke()
        {
            #region Kpi_1

            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);

            // ✅ Saat bilgisi varsa == tutmaz. Range en sağlıklısı (SQL'e her zaman çevrilir)
            var todayOrderCount = _context.Orders.Count(x => x.OrderDate >= today && x.OrderDate < today.AddDays(1));
            var yesterdayOrderCount = _context.Orders.Count(x => x.OrderDate >= yesterday && x.OrderDate < yesterday.AddDays(1));

            ViewBag.TrendingIcon = (todayOrderCount > yesterdayOrderCount)
                ? "zmdi zmdi-trending-up float-right"
                : "zmdi zmdi-trending-down float-right";

            // ✅ Senin istediğin mantık: veri yoksa hesaplama yapma
            decimal changeRate = 0;
            if (todayOrderCount >= 1 && yesterdayOrderCount >= 1) // payda 0 olmasın
            {
                changeRate = ((decimal)(todayOrderCount - yesterdayOrderCount) / yesterdayOrderCount) * 100m;
            }

            ViewBag.DailyOrderChange = Math.Round(changeRate, 2);
            ViewBag.ChangeRateColor = changeRate < 0 ? "red" : "green";

            // ✅ EF translate issue yaşamamak için günlük sayıları çek -> ortalamayı C# hesapla
            var dailyCounts = _context.Orders
                .GroupBy(x => x.OrderDate.Date)
                .Select(g => g.Count())
                .ToList();

            double dailyAverageOrders = dailyCounts.Count > 0 ? dailyCounts.Average() : 0;

            double ratio = 0;
            if (todayOrderCount >= 1 && dailyAverageOrders > 0) // payda 0 olmasın
            {
                ratio = (todayOrderCount / dailyAverageOrders) * 100.0;
            }

            ViewBag.TodayVsAverageRatio = Math.Round(ratio, 2);
            ViewBag.TodayOrderCount = todayOrderCount;

            #endregion

            #region Kpi_2

            var sevenDaysAgo = today.AddDays(-7);

            var totalOrders7Days = _context.Orders.Count(x =>
                x.OrderDate >= sevenDaysAgo && x.OrderDate < today.AddDays(1));

            var cancelledOrders7Days = _context.Orders.Count(x =>
                x.OrderStatus == "İptal Edildi"
                && x.OrderDate >= sevenDaysAgo
                && x.OrderDate < today.AddDays(1));

            decimal cancelRate = 0;
            if (totalOrders7Days >= 1) // payda 0 olmasın
            {
                cancelRate = ((decimal)cancelledOrders7Days / totalOrders7Days) * 100m;
            }

            ViewBag.CancelledOrders7Days = cancelledOrders7Days;
            ViewBag.CancelRate = Math.Round(cancelRate, 2);
            ViewBag.CancelColor = "red";
            ViewBag.CancelText = cancelRate > 5 ? "Yüksek İptal Oranı ⚠️" : "Normal Düzeyde";

            #endregion

            #region Kpi_3

            var totalOrders = _context.Orders.Count();
            var completedOrders = _context.Orders.Count(x => x.OrderStatus == "Tamamlandı");

            decimal completionRate = 0;
            if (totalOrders >= 1) // payda 0 olmasın
            {
                completionRate = ((decimal)completedOrders / totalOrders) * 100m;
            }

            ViewBag.CompletionRate = Math.Round(completionRate, 2);
            ViewBag.CompletedOrders = completedOrders;
            ViewBag.CompletionText = completionRate >= 80
                ? "Mükemmel Performans 💪"
                : "İyileşme Devam Ediyor 📈";

            #endregion

            return View();
        }
    }
}
