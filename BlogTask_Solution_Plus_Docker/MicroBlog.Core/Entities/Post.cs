using System;

namespace MicroBlog.Core.Entities
{
    public class Post
    {
        private string _text;
        private double _lat, _lng;

        public int Id { get; set; }
        public string UserName { get; set; }
        public string Text {
            get => _text;
            set {
                if (value.Length > 140)
                    throw new ArgumentException("Text must be maximum 140 characters.");
                _text = value;
            }
        }
        public string OriginalImageUrl { get; set; }
        public string WebPImageUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public double Latitude {
            get => _lat;
            set {
                if (value < -90 || value > 90)
                    throw new ArgumentOutOfRangeException(nameof(Latitude));
                _lat = value;
            }
        }
        public double Longitude {
            get => _lng;
            set {
                if (value < -180 || value > 180)
                    throw new ArgumentOutOfRangeException(nameof(Longitude));
                _lng = value;
            }
        }

        //public bool IsImageProcessed => !string.IsNullOrEmpty(WebPImageUrl);

        //private bool _isImagePRocessed
        public bool IsImageProcessed {
            get { return !string.IsNullOrEmpty(WebPImageUrl); }
           
        }

    }
}
