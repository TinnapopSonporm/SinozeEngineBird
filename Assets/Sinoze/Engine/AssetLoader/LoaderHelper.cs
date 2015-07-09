
public class LoaderHelper  {

	public static string CheckPath(string path)
	{
		string newPath = "file:///";
		if (!path.Contains("http") && !path.Contains("file"))
		{
			path = newPath + path;
		}
		return path; 
	}
}
