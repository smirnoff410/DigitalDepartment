using ObjectDetectMobile.Services;
using System.Reflection;
using System.Text.Json;

namespace ObjectDetectMobile;

public partial class MainPage : ContentPage
{
    UploadImage uploadImage { get; set; }

    public MainPage()
    {
        InitializeComponent();
        uploadImage = new UploadImage();
    }

    private async void UploadImage_Clicked(object sender, EventArgs e)
    {
        var img = await uploadImage.OpenMediaPickerAsync();

        //Если изображение не выбрано, то используем тестовые данные
        if (img == null)
        {
            //Assembly assembly = GetType().GetTypeInfo().Assembly;
            //using Stream stream = assembly.GetManifestResourceStream("ObjectDetectMobile.Resources.Images.people.jpg");
            //imageStream = stream;
            UploadedImage.Source = "people.jpg";

            var fakeData = new Response
            {
                Height = 400,
                Width = 400,
                Result = new List<Result>
                {
                    new Result { Classification = "Person", Coordinate = new List<Coordinate>{ new Coordinate { X = "120", Y = "50" }, new Coordinate { X = "150", Y = "70" } }},
                    new Result { Classification = "Person", Coordinate = new List<Coordinate>{ new Coordinate { X = "170", Y = "80" }, new Coordinate { X = "200", Y = "90" } }},
                    new Result { Classification = "Person", Coordinate = new List<Coordinate>{ new Coordinate { X = "210", Y = "100" }, new Coordinate { X = "220", Y = "105" } }},
                    new Result { Classification = "Person", Coordinate = new List<Coordinate>{ new Coordinate { X = "230", Y = "110" }, new Coordinate { X = "240", Y = "120" } }},
                    new Result { Classification = "Person", Coordinate = new List<Coordinate>{ new Coordinate { X = "250", Y = "130" }, new Coordinate { X = "260", Y = "140" } }},
                }
            };

            foreach(var image in fakeData.Result)
            {
                var x1 = image.Coordinate.First().X;
                var y1 = image.Coordinate.First().Y;
                var x2 = image.Coordinate.Last().X;
                var y2 = image.Coordinate.Last().Y;
                DetectedObjects.Text += $"{image.Classification} X1:{x1},Y1:{y1},X2:{x2},Y2:{y2}\n";
            }
        }
        else
        {
            var imageStream = await img.OpenReadAsync();
            var httpClient = new HttpClient();

            StreamContent content = new StreamContent(imageStream);
            //Отправляем запрос на сервер, чтобы он сохранил изображение
            var uploadImageResponse = await httpClient.PostAsync("http://10.0.2.2:5000/upload_image", content);
            if (!uploadImageResponse.IsSuccessStatusCode)
            {
                return;
            }
            var uploadImageResult = await uploadImageResponse.Content.ReadAsStringAsync();
            var fileName = JsonSerializer.Deserialize<UploadImageResponse>(uploadImageResult).FileName;

            //Делаем запрос на обнаружение объектов
            var response = await httpClient.GetAsync($"http://10.0.2.2:8000/{fileName}");
            if (!response.IsSuccessStatusCode)
                return;

            var result = response.Content.ReadAsStringAsync().Result;
            var deserializeResult = JsonSerializer.Deserialize<Response>(result);

            foreach (var image in deserializeResult.Result)
            {
                var x1 = image.Coordinate.First().X;
                var y1 = image.Coordinate.First().Y;
                var x2 = image.Coordinate.Last().X;
                var y2 = image.Coordinate.Last().Y;
                DetectedObjects.Text += $"{image.Classification} X1:{x1},Y1:{y1},X2:{x2},Y2:{y2}\n";
            }
        }

        /*Image_Upload.Source = ImageSource.FromStream(() =>
            uploadImage.ByteArrayToStream(uploadImage.StringToByteBase64(imagefile.byteBase64))
        );*/
    }

    public class UploadImageResponse
    {
        public string FileName { get; set; }
    }

    public class Coordinate
    {
        public string X { get; set; }
        public string Y { get; set; }
    }

    public class Result
    {
        public string Classification { get; set; }
        public List<Coordinate> Coordinate { get; set; }
    }

    public class Response
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public List<Result> Result { get; set; }
    }
}

