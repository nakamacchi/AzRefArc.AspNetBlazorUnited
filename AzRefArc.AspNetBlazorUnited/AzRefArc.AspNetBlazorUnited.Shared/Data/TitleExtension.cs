using System.ComponentModel.DataAnnotations.Schema;

namespace AzRefArc.AspNetBlazorUnited.Shared.Data
{
    public partial class Title
    {
        [NotMapped]
        public string ImageName
        {
            get
            {
                string[] files = { "animal_01.jpg", "animal_02.jpg", "animal_03.jpg", "animal_04.jpg", "animal_05.jpg", "animal_06.jpg", "animal_07.jpg", "animal_08.jpg", "cat_01.jpg", "cat_02.jpg", "cat_03.jpg", "cat_04.jpg", "cat_05.jpg", "cat_06.jpg", "cat_07.jpg", "cat_08.jpg", "cat_09.jpg", "cat_10.jpg", "cat_11.jpg", "cat_12.jpg", "sky_01.jpg", "sky_02.jpg", "sky_03.jpg", "sky_04.jpg", "sky_05.jpg", "sky_06.jpg", "sky_07.jpg", "sky_08.jpg", "sky_09.jpg", "sky_10.jpg", "sky_11.jpg", "sky_12.jpg", "sky_13.jpg", "sky_14.jpg", "sky_15.jpg" };
                return files[Math.Abs(TitleId.GetHashCode()) % files.Length];
            }
        }

        [NotMapped]
        public string ImageUrl
        {
            get { return "/Images/Samples/" + ImageName; }
        }

        [NotMapped]
        public string ImageThumbnailUrl
        {
            get { return "/Images/Samples/Thumbnails/" + ImageName; }
        }
    }
}
