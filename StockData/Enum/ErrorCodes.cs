namespace StockData.Enum
{
    public class ErrorCodes
    {
        public static ErrorModel UnkownError { get { return new ErrorModel { Code = 500, Text = "Bilinmeyen bir hata olustu!" }; } }
        public static ErrorModel WrongVariant { get { return new ErrorModel { Code = 400, Text = "Bu variant code başka bir ürüne aitdir." }; } }

    }

    public class ErrorModel
    {
        public int Code { get; set; }
        public string Text { get; set; }
    }
}
