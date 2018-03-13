using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public abstract class ReportMaker
	{
		protected AbstractCaption AbstractCaption;
		protected AbstractList AbstractList;
		protected AbstractItem AbstractItem;
		protected AbstractStatistic AbstractStatistic;

		public string MakeReport(IEnumerable<Measurement> measurements)
		{
			var data = measurements.ToList();
			var result = new StringBuilder();
			result.Append(AbstractCaption.MakeCaption());
			result.Append(AbstractList.BeginList());
			result.Append(AbstractItem.MakeItem("Temperature", AbstractStatistic.MakeStatistics(data.Select(z => z.Temperature)).ToString()));
			result.Append(AbstractItem.MakeItem("Humidity", AbstractStatistic.MakeStatistics(data.Select(z => z.Humidity)).ToString()));
			result.Append(AbstractList.EndList());
			return result.ToString();
		}
	}

	#region Caption

	public abstract class AbstractCaption
	{
		public string Caption { get; set; }
		public abstract string MakeCaption();
	}

	public class HTMLCaption : AbstractCaption
	{
		public HTMLCaption(string Caption)
		{
			this.Caption = Caption;
		}
		public override string MakeCaption()
		{
			return $"<h1>{Caption}</h1>";
		}
	}

	public class MarkdownCaption : AbstractCaption
	{
		public MarkdownCaption(string Caption)
		{
			this.Caption = Caption;
		}
		public override string MakeCaption()
		{
			return $"## {Caption}\n\n";
		}
	}
	#endregion

	#region Statistic

	public abstract class AbstractStatistic
	{
		public abstract object MakeStatistics(IEnumerable<double> _data);
	}

	public class MeanAndStdStatistic : AbstractStatistic
	{
		public override object MakeStatistics(IEnumerable<double> _data)
		{
			var data = _data.ToList();
			var mean = data.Average();
			var std = Math.Sqrt(data.Select(z => Math.Pow(z - mean, 2)).Sum() / (data.Count - 1));

			return new MeanAndStd
			{
				Mean = mean,
				Std = std
			};
		}
	}

	public class MarkdownStatistic : AbstractStatistic
	{
		public override object MakeStatistics(IEnumerable<double> data)
		{
			var list = data.OrderBy(z => z).ToList();
			if (list.Count % 2 == 0)
				return (list[list.Count / 2] + list[list.Count / 2 - 1]) / 2;
			else
				return list[list.Count / 2];
		}
	}

	#endregion

	#region List

	public abstract class AbstractList
	{
		public abstract string BeginList();
		public abstract string EndList();
	}

	public class HTMLList : AbstractList
	{
		public override string BeginList()
		{
			return "<ul>";
		}

		public override string EndList()
		{
			return "</ul>";
		}
	}

	public class MarkdownList : AbstractList
	{
		public override string BeginList()
		{
			return string.Empty;
		}

		public override string EndList()
		{
			return string.Empty;
		}
	}

	#endregion

	#region Item

	public abstract class AbstractItem
	{
		public abstract string MakeItem(string valueType, string Entry);
	}

	public class HTMLItem : AbstractItem
	{
		public override string MakeItem(string ValueType, string Entry)
		{
			return $"<li><b>{ValueType}</b>: {Entry}";
		}
	}

	public class MarkdownItem : AbstractItem
	{
		public override string MakeItem(string ValueType, string Entry)
		{
			return $" * **{ValueType}**: {Entry}\n\n";
		}
	}

	#endregion


	public class MeanAndStdHtmlReportMaker : ReportMaker
	{
		public MeanAndStdHtmlReportMaker()
		{
			AbstractCaption = new HTMLCaption("Mean and Std");
			AbstractList = new HTMLList();
			AbstractStatistic = new MeanAndStdStatistic();
			AbstractItem = new HTMLItem();
		}
	}

	public class MedianMarkdownReportMaker : ReportMaker
	{
		public MedianMarkdownReportMaker()
		{
			AbstractCaption = new MarkdownCaption("Median");
			AbstractList = new MarkdownList();
			AbstractStatistic = new MarkdownStatistic();
			AbstractItem = new MarkdownItem();
		}
	}

	public class MeanAndStdMarkdownReportMaker : ReportMaker
	{
		public MeanAndStdMarkdownReportMaker()
		{
			AbstractCaption = new MarkdownCaption("Mean and Std");
			AbstractList = new MarkdownList();
			AbstractStatistic = new MeanAndStdStatistic();
			AbstractItem = new MarkdownItem();
		}
	}

	public class MedianHtmlReportMaker : ReportMaker
	{
		public MedianHtmlReportMaker()
		{
			AbstractCaption = new HTMLCaption("Median");
			AbstractList = new HTMLList();
			AbstractStatistic = new MarkdownStatistic();
			AbstractItem = new HTMLItem();
		}
	}

	public static class ReportMakerHelper
	{
		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			return new MeanAndStdHtmlReportMaker().MakeReport(data);
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
			return new MedianMarkdownReportMaker().MakeReport(data);
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
			return new MeanAndStdMarkdownReportMaker().MakeReport(measurements);
		}

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
			return new MedianHtmlReportMaker().MakeReport(measurements);
		}
	}
}
