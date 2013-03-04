//      *********    请勿修改此文件     *********
//      此文件由设计工具再生成。更改
//      此文件可能会导致错误。
namespace Expression.Blend.SampleData.SampleDataSource
{
	using System; 

// To significantly reduce the sample data footprint in your production application, you can set
// the DISABLE_SAMPLE_DATA conditional compilation constant and disable sample data at runtime.
#if DISABLE_SAMPLE_DATA
	internal class SampleDataSource { }
#else

	public class SampleDataSource : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		public SampleDataSource()
		{
			try
			{
				System.Uri resourceUri = new System.Uri("/WinDou;component/SampleData/SampleDataSource/SampleDataSource.xaml", System.UriKind.Relative);
				if (System.Windows.Application.GetResourceStream(resourceUri) != null)
				{
					System.Windows.Application.LoadComponent(this, resourceUri);
				}
			}
			catch (System.Exception)
			{
			}
		}

		private ItemCollection _Collection = new ItemCollection();

		public ItemCollection Collection
		{
			get
			{
				return this._Collection;
			}
		}

		private AllTopicList _AllTopicList = new AllTopicList();

		public AllTopicList AllTopicList
		{
			get
			{
				return this._AllTopicList;
			}
		}

		private GroupTopicReviewList _GroupTopicReviewList = new GroupTopicReviewList();

		public GroupTopicReviewList GroupTopicReviewList
		{
			get
			{
				return this._GroupTopicReviewList;
			}
		}

		private CurrentTopic _CurrentTopic = new CurrentTopic();

		public CurrentTopic CurrentTopic
		{
			get
			{
				return this._CurrentTopic;
			}

			set
			{
				if (this._CurrentTopic != value)
				{
					this._CurrentTopic = value;
					this.OnPropertyChanged("CurrentTopic");
				}
			}
		}

		private ReviewList _ReviewList = new ReviewList();

		public ReviewList ReviewList
		{
			get
			{
				return this._ReviewList;
			}
		}

		private string _Property1 = string.Empty;

		public string Property1
		{
			get
			{
				return this._Property1;
			}

			set
			{
				if (this._Property1 != value)
				{
					this._Property1 = value;
					this.OnPropertyChanged("Property1");
				}
			}
		}
	}

	public class ItemCollection : System.Collections.ObjectModel.ObservableCollection<Item>
	{ 
	}

	public class Item : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Property1 = string.Empty;

		public string Property1
		{
			get
			{
				return this._Property1;
			}

			set
			{
				if (this._Property1 != value)
				{
					this._Property1 = value;
					this.OnPropertyChanged("Property1");
				}
			}
		}

		private bool _Property2 = false;

		public bool Property2
		{
			get
			{
				return this._Property2;
			}

			set
			{
				if (this._Property2 != value)
				{
					this._Property2 = value;
					this.OnPropertyChanged("Property2");
				}
			}
		}
	}

	public class AllTopicList : System.Collections.ObjectModel.ObservableCollection<AllTopicListItem>
	{ 
	}

	public class AllTopicListItem : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private Group _Group = new Group();

		public Group Group
		{
			get
			{
				return this._Group;
			}

			set
			{
				if (this._Group != value)
				{
					this._Group = value;
					this.OnPropertyChanged("Group");
				}
			}
		}

		private string _Title = string.Empty;

		public string Title
		{
			get
			{
				return this._Title;
			}

			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.OnPropertyChanged("Title");
				}
			}
		}

		private string _CommentsCount = string.Empty;

		public string CommentsCount
		{
			get
			{
				return this._CommentsCount;
			}

			set
			{
				if (this._CommentsCount != value)
				{
					this._CommentsCount = value;
					this.OnPropertyChanged("CommentsCount");
				}
			}
		}

		private string _Created = string.Empty;

		public string Created
		{
			get
			{
				return this._Created;
			}

			set
			{
				if (this._Created != value)
				{
					this._Created = value;
					this.OnPropertyChanged("Created");
				}
			}
		}
	}

	public class Group : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}

		private string _UId = string.Empty;

		public string UId
		{
			get
			{
				return this._UId;
			}

			set
			{
				if (this._UId != value)
				{
					this._UId = value;
					this.OnPropertyChanged("UId");
				}
			}
		}
	}

	public class GroupTopicReviewList : System.Collections.ObjectModel.ObservableCollection<GroupTopicReviewListItem>
	{ 
	}

	public class GroupTopicReviewListItem : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private Author _Author = new Author();

		public Author Author
		{
			get
			{
				return this._Author;
			}

			set
			{
				if (this._Author != value)
				{
					this._Author = value;
					this.OnPropertyChanged("Author");
				}
			}
		}

		private string _Time = string.Empty;

		public string Time
		{
			get
			{
				return this._Time;
			}

			set
			{
				if (this._Time != value)
				{
					this._Time = value;
					this.OnPropertyChanged("Time");
				}
			}
		}

		private string _Text = string.Empty;

		public string Text
		{
			get
			{
				return this._Text;
			}

			set
			{
				if (this._Text != value)
				{
					this._Text = value;
					this.OnPropertyChanged("Text");
				}
			}
		}
	}

	public class Author : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
	}

	public class CurrentTopic : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Title = string.Empty;

		public string Title
		{
			get
			{
				return this._Title;
			}

			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.OnPropertyChanged("Title");
				}
			}
		}
	}

	public class ReviewList : System.Collections.ObjectModel.ObservableCollection<ReviewListItem>
	{ 
	}

	public class ReviewListItem : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Summary = string.Empty;

		public string Summary
		{
			get
			{
				return this._Summary;
			}

			set
			{
				if (this._Summary != value)
				{
					this._Summary = value;
					this.OnPropertyChanged("Summary");
				}
			}
		}

		private string _Title = string.Empty;

		public string Title
		{
			get
			{
				return this._Title;
			}

			set
			{
				if (this._Title != value)
				{
					this._Title = value;
					this.OnPropertyChanged("Title");
				}
			}
		}

		private string _Updated = string.Empty;

		public string Updated
		{
			get
			{
				return this._Updated;
			}

			set
			{
				if (this._Updated != value)
				{
					this._Updated = value;
					this.OnPropertyChanged("Updated");
				}
			}
		}

		private Author1 _Author = new Author1();

		public Author1 Author
		{
			get
			{
				return this._Author;
			}

			set
			{
				if (this._Author != value)
				{
					this._Author = value;
					this.OnPropertyChanged("Author");
				}
			}
		}
	}

	public class Author1 : System.ComponentModel.INotifyPropertyChanged
	{
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			if (this.PropertyChanged != null)
			{
				this.PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
			}
		}

		private string _Name = string.Empty;

		public string Name
		{
			get
			{
				return this._Name;
			}

			set
			{
				if (this._Name != value)
				{
					this._Name = value;
					this.OnPropertyChanged("Name");
				}
			}
		}
	}
#endif
}
