using MongoDB.Bson;


public class DiyCardInfo
{
    public ObjectId _id;
    public int uid;
    public string cardName;
    public string describe;
    public string imageUrl;
    public List<Commit> commits = new List<Commit>();
    public class Commit
    {
        public string user;
        public string text;
        public List<string> likeList = new List<string>();
        public List<string> dislikeList = new List<string>();
    }
}
public class DefaultTexture
{
    public ObjectId _id;
    public byte[] cardFramesImage;
    public byte[] cardUploadImage;
    public byte[] cardLoadImage;
}
//public class DiyCardTextureInfo
//{

//    public ObjectId _id;
//    public int uid;
//    public byte[] imagedata;
//	public Image GetImage() => Image.FromStream(new MemoryStream(imagedata));
//}
//public class ResponseModel
//{
//    public string name { get; set; }

//    public string status { get; set; }

//    public string url { get; set; }

//    public string thumbUrl { get; set; }
//}