namespace WeatherApp;

using System.Net.Http;
using Newtonsoft.Json.Linq;

public partial class WeatherForm : Form
{
    private TextBox cityInput = null!;
private Button searchBtn = null!;
private Label cityLabel = null!;
private Label tempLabel = null!;
private Label descLabel = null!;
private Label humidityLabel = null!;
private Label windLabel = null!;
private Panel mainPanel = null!;
private readonly HttpClient client = new HttpClient();
    private const string API_KEY = "350cfc9570c1e7fa4a0c1dde35259781";

    public WeatherForm()
    {
        InitializeComponents();
    }

    private void InitializeComponents()
        {
            // Form setup
            this.Text = "Weather App";
            this.Size = new Size(420, 580);
            this.BackColor = Color.FromArgb(13, 13, 13);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Font = new Font("Consolas", 10);

            // Main panel
            mainPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(13, 13, 13),
                Padding = new Padding(32)
            };

            // City input
            cityInput = new TextBox
            {
                Location = new Point(32, 40),
                Size = new Size(260, 36),
                BackColor = Color.FromArgb(22, 22, 22),
                ForeColor = Color.FromArgb(232, 226, 217),
                BorderStyle = BorderStyle.FixedSingle,
                Font = new Font("Consolas", 11),
                PlaceholderText = "Enter city..."
            };

            // Search button
            searchBtn = new Button
            {
                Text = "Search",
                Location = new Point(304, 40),
                Size = new Size(80, 36),
                BackColor = Color.FromArgb(200, 240, 96),
                ForeColor = Color.FromArgb(13, 13, 13),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Consolas", 10, FontStyle.Bold),
                Cursor = Cursors.Hand
            };
            searchBtn.FlatAppearance.BorderSize = 0;
            searchBtn.Click += async (s, e) => await FetchWeather();

            // City name label
            cityLabel = new Label
            {
                Location = new Point(32, 120),
                Size = new Size(360, 48),
                ForeColor = Color.FromArgb(232, 226, 217),
                Font = new Font("Georgia", 28, FontStyle.Italic),
                Text = "—"
            };

            // Temperature label
            tempLabel = new Label
            {
                Location = new Point(32, 180),
                Size = new Size(360, 80),
                ForeColor = Color.FromArgb(200, 240, 96),
                Font = new Font("Georgia", 56, FontStyle.Bold),
                Text = ""
            };

            // Description label
            descLabel = new Label
            {
                Location = new Point(32, 268),
                Size = new Size(360, 32),
                ForeColor = Color.FromArgb(102, 102, 102),
                Font = new Font("Consolas", 11),
                Text = ""
            };

            // Divider
            var divider = new Panel
            {
                Location = new Point(32, 316),
                Size = new Size(360, 1),
                BackColor = Color.FromArgb(37, 37, 37)
            };

            // Humidity label
            humidityLabel = new Label
            {
                Location = new Point(32, 332),
                Size = new Size(360, 28),
                ForeColor = Color.FromArgb(102, 102, 102),
                Font = new Font("Consolas", 10),
                Text = ""
            };

            // Wind label
            windLabel = new Label
            {
                Location = new Point(32, 364),
                Size = new Size(360, 28),
                ForeColor = Color.FromArgb(102, 102, 102),
                Font = new Font("Consolas", 10),
                Text = ""
            };

            // Add controls
            this.Controls.AddRange(new Control[] {
                cityInput, searchBtn, cityLabel,
                tempLabel, descLabel, divider,
                humidityLabel, windLabel
            });

            // Enter key triggers search
            cityInput.KeyPress += async (s, e) => {
                if (e.KeyChar == (char)Keys.Enter)
                    await FetchWeather();
            };
        }

        private async Task FetchWeather()
        {
            string city = cityInput.Text.Trim();
            if (string.IsNullOrEmpty(city)) return;

            searchBtn.Enabled = false;
            searchBtn.Text = "...";

            try
            {
                string url = $"https://api.openweathermap.org/data/2.5/weather?q={city}&appid={API_KEY}&units=metric";
                string response = await client.GetStringAsync(url);
                var data = JObject.Parse(response);

                string name = data["name"]?.ToString() ?? city;
                string country = data["sys"]?["country"]?.ToString() ?? "";
                double temp = data["main"]?["temp"]?.Value<double>() ?? 0;
                string desc = data["weather"]?[0]?["description"]?.ToString() ?? "";
                int humidity = data["main"]?["humidity"]?.Value<int>() ?? 0;
                double wind = data["wind"]?["speed"]?.Value<double>() ?? 0;

                // Animate temperature change
                await AnimateTemperature((int)temp);

                cityLabel.Text = $"{name}, {country}";
                descLabel.Text = char.ToUpper(desc[0]) + desc.Substring(1);
                humidityLabel.Text = $"HUMIDITY    {humidity}%";
                windLabel.Text = $"WIND        {wind} m/s";
            }
            catch (Exception)
            {
                cityLabel.Text = "City not found";
                tempLabel.Text = "";
                descLabel.Text = "";
                humidityLabel.Text = "";
                windLabel.Text = "";
            }
            finally
            {
                searchBtn.Enabled = true;
                searchBtn.Text = "Search";
            }
        }

        private async Task AnimateTemperature(int targetTemp)
        {
            int current = 0;
            int step = targetTemp > 0 ? 1 : -1;

            while (current != targetTemp)
            {
                current += step;
                tempLabel.Text = $"{current}°C";
                await Task.Delay(20);
            }
        }

}