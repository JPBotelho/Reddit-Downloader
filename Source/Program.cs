using System;
using System.Linq;
using RedditSharp;
using System.Net;
using RedditSharp.Things;
using System.Security.Authentication;
using System.Collections.Generic;

class Program
{
	public static void Main(string[] args)
	{
		if (args.Length < 3) return;

		Console.WriteLine("--- COMMENCING SCRAPING ---");

		Reddit reddit = new Reddit();
		Subreddit subreddit = null;
		int takes = 0;
		try
		{
			subreddit = reddit.GetSubreddit("/r/" + args[0]);
			takes = Convert.ToInt32(args[2]);
		} 
		catch
		{
			Console.WriteLine("ERROR: Invalid Sub");
			Console.ReadKey();
			return;
		}

		Listing<Post> posts = GetPostList(args[1], subreddit);

		if(posts == null)
		{
			Console.WriteLine("ERROR: FAILED TO FETCH POSTS");
			Console.ReadKey();
			return;
		}

		foreach (Post post in posts.Take(takes))
		{
			Console.WriteLine("\n" + post.Url);

			string extension = System.IO.Path.GetExtension(post.Url.ToString());

			bool isFile = (extension == ".png" || extension == ".jpg" || extension == ".jpeg" || extension == ".gif");

			Console.WriteLine(isFile);
			if (isFile)
			{
				var downloader = new System.Net.WebClient();
				downloader.DownloadFile(post.Url.ToString(), post.FullName + extension);
			}

		}
		Console.WriteLine("--- SCRAPING COMPLETED ---");
		Console.ReadKey();
	}

	static Listing<Post> GetPostList(string arg, Subreddit subreddit)
	{
		Listing<Post> postList;

		switch(arg.ToLower())
		{
			case "hot":
				postList = subreddit.Hot;
				break;
			case "top":
				postList = subreddit.GetTop(FromTime.All);
				break;
			case "new":
				postList = subreddit.New;
				break;
			case "controversial":
				postList = subreddit.Controversial;
				break;
			default:
				return null;
		}

		return postList;
	}
}
