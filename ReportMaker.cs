using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Delegates.Reports
{
	public class ReportMaker
	{
		public AbstractReport Report { get; set; }

		public ReportMaker(AbstractReport Report)
		{
			this.Report = Report;
		}

		public string MakeReport(IEnumerable<Measurement> measurements)
		{
			var data = measurements.ToList();
			var result = new StringBuilder();
			result.Append(Report.MakeCaption());
			result.Append(Report.ReportStartList);
			result.Append(Report.MakeItem("Temperature", Report.GetStatistic(data.Select(z => z.Temperature)).ToString()));
			result.Append(Report.MakeItem("Humidity", Report.GetStatistic(data.Select(z => z.Humidity)).ToString()));
			result.Append(Report.ReportEndList);
			return result.ToString();
		}
	}

	#region Report

	public abstract class AbstractReport
	{
		public string Caption { get; protected set; }
		public string ReportStartList { get; protected set; }
		public string ReportEndList { get; protected set; }
		public AbstractStatistic ReportStatistic { get; protected set; }

		public abstract string MakeCaption();
		public abstract string MakeItem(string ValueType, string Entry);
		public virtual object GetStatistic(IEnumerable<double> data)
		{
			return ReportStatistic.MakeStatistics(data);
		}
	}

	public class HTMLReport : AbstractReport
	{

		public HTMLReport(string Caption, AbstractStatistic Statistic)
		{
			this.Caption = Caption;
			ReportStartList = "<ul>";
			ReportEndList = "</ul>";
			ReportStatistic = Statistic;
		}

		public override string MakeCaption()
		{
			return $"<h1>{Caption}</h1>";
		}

		public override string MakeItem(string ValueType, string Entry)
		{
			return $"<li><b>{ValueType}</b>: {Entry}";
		}
	}

	public class MarkdownReport : AbstractReport
	{
		public MarkdownReport(string Caption, AbstractStatistic Statistic)
		{
			this.Caption = Caption;
			ReportStartList = string.Empty;
			ReportEndList = string.Empty;
			this.ReportStatistic = Statistic;
		}

		public override string MakeCaption()
		{
			return $"## {Caption}\n\n";
		}

		public override string MakeItem(string ValueType, string Entry)
		{
			return $" * **{ValueType}**: {Entry}\n\n";
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

	public static class ReportMakerHelper
	{
		private static ReportMaker ReportMaker;

		public static string MeanAndStdHtmlReport(IEnumerable<Measurement> data)
		{
			ReportMaker = new ReportMaker(new HTMLReport("Mean and Std", new MeanAndStdStatistic()));
			return ReportMaker.MakeReport(data);
		}

		public static string MedianMarkdownReport(IEnumerable<Measurement> data)
		{
			ReportMaker = new ReportMaker(new MarkdownReport("Median", new MarkdownStatistic()));
			return ReportMaker.MakeReport(data);
		}

		public static string MeanAndStdMarkdownReport(IEnumerable<Measurement> measurements)
		{
			ReportMaker = new ReportMaker(new MarkdownReport("Mean and Std", new MeanAndStdStatistic()));
			return ReportMaker.MakeReport(measurements);
		}

		public static string MedianHtmlReport(IEnumerable<Measurement> measurements)
		{
			ReportMaker = new ReportMaker(new HTMLReport("Median", new MarkdownStatistic()));
			return ReportMaker.MakeReport(measurements);
		}
	}
}
