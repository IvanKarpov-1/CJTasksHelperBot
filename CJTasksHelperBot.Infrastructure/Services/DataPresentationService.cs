using System.Collections.ObjectModel;
using System.Text;
using Alba.CsConsoleFormat;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Domain.Enums;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;

namespace CJTasksHelperBot.Infrastructure.Services;

public class DataPresentationService : IDataPresentationService
{
	private static readonly LineThickness NoStroke = LineThickness.None;

	private readonly IStringLocalizer<Messages> _localizer;

	public DataPresentationService(IStringLocalizer<Messages> localizer)
	{
		_localizer = localizer;
	}

	public string GetTabledTextRepresentation(IEnumerable<GetTaskDto> items)
	{
		var columns = new Collection<GridLength>
		{
			GridLength.Auto,
			GridLength.Char(15),
			GridLength.Char(11),
			GridLength.Char(25),
		};

		var headers = new List<Cell>
		{
			new(" №") { Padding = new Thickness(0, 0, 1, 0) },
			new(_localizer["table_cell_name"]) { Padding = new Thickness(0, 0, 1, 0) },
			new(_localizer["table_cell_deadline"]),
			new(_localizer["table_cell_description"]) { Padding = new Thickness(0, 0, 1, 0) },
		};
		
		var table = BuildTable(items, columns, headers, BuildRow);
		
		return RenderTableToString(new Document(table));

		Cell[] BuildRow(GetTaskDto task) => 
		[
			new Cell(task.Title) { Stroke = NoStroke }, 
			new Cell(GetFormattedDateTime(task.Deadline)) { Stroke = NoStroke }, 
			new Cell(task.Description) { Stroke = NoStroke }, 
		];
	}

	public string GetTabledTextRepresentation(IEnumerable<UserTaskStatusDto> items)
	{
		var columns = new Collection<GridLength>
		{
			GridLength.Auto,
			GridLength.Char(15),
			GridLength.Char(11),
			GridLength.Char(25),
			GridLength.Char(20),
		};

		var headers = new List<Cell>
		{
			new(" №") { Padding = new Thickness(0, 0, 1, 0) },
			new(_localizer["table_cell_name"]) { Padding = new Thickness(0, 0, 1, 0) },
			new(_localizer["table_cell_deadline"]),
			new(_localizer["table_cell_description"]) { Padding = new Thickness(0, 0, 1, 0) },
			new(_localizer["table_cell_status"]) { Align = Align.Left },
		};
		
		var table = BuildTable(items, columns, headers, BuildRow);
		
		return RenderTableToString(new Document(table));

		Cell[] BuildRow(UserTaskStatusDto task) => 
		[
			new Cell(task.Task!.Title), 
			new Cell(GetFormattedDateTime(task.Task.Deadline)), 
			new Cell(task.Task.Description), 
			new Cell(_localizer[TaskStatusCustomEnum.FromValue((int)task.TaskStatus).DisplayName]) { Align = Align.Left },
		];
	}

	public string GetPlainTextRepresentation(IEnumerable<GetTaskDto> items)
	{
		var tasks = items.ToList();
		var tasksInfo = BuildPlainText(tasks, task => 
			$"{task.Title} | " +
			$"{_localizer["word_by"]}: {GetFormattedDateTime(task.Deadline)} | " +
			$"{task.Description} |");
		return tasksInfo.ToString();
	}
	
	public string GetPlainTextRepresentation(IEnumerable<UserTaskStatusDto> items)
	{
		var tasks = items.ToList();
		var tasksInfo = BuildPlainText(tasks, task => 
			$"{task.Task!.Title} | " +
			$"{_localizer["word_by"]}: {GetFormattedDateTime(task.Task.Deadline)} | " +
			$"{task.Task.Description} | " +
			$"{_localizer["word_status"]}: {_localizer[TaskStatusCustomEnum.FromValue((int)task.TaskStatus).DisplayName]};");
		return tasksInfo.ToString();
	}
	
	private static Grid BuildTable<T>(IEnumerable<T> items, Collection<GridLength> columns, List<Cell> headers, Func<T, Cell[]> buildRow)
	{
		var i = 1;
		return new Grid
		{
			Columns =
			{
				columns,
			},
			Stroke = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.None, LineWidth.None),
			Children =
			{
				headers.Select(cell =>
				{
					cell.Stroke = NoStroke;
					return cell;
				}),
				headers.Select(_ => new Cell(" ") {Stroke = NoStroke}),
				items
					.Select(item => buildRow(item)
						.Prepend(new Cell($" {i++:00}") {Padding = new Thickness(0, 0, 1, 0)}))
					.Select(x => x.Select(cell =>
					{
						cell.Stroke = NoStroke;
						return cell;
					}))
			}
		};
	}
	
	private static string RenderTableToString(Document document)
	{
		var textRenderTarget = new TextRenderTarget();
		ConsoleRenderer.RenderDocumentToText(document, textRenderTarget, ConsoleRenderRect);

		using var sr = new StringReader(textRenderTarget.OutputText);
		var table = new StringBuilder();

		while (sr.ReadLine() is { } line)
		{
			table.AppendLine(line.TrimEnd());
		}

		return table.ToString();
	}
	
	private static Rect ConsoleRenderRect
	{
		get
		{
			const int width = 100;

			try {
				return Console.BufferWidth != 0 
					? ConsoleRenderer.DefaultRenderRect
					: new Rect(0, 0, width, int.MaxValue);
			}
			catch {
				return new Rect(0, 0, width, int.MaxValue);
			}
		}
	}

	private StringBuilder BuildPlainText<T>(IEnumerable<T> items, Func<T, string> buildRow)
	{
		var tasks = items.ToList();
		
		var tasksInfo = new StringBuilder();
		var i = 1;

		if (tasks.Count != 0)
		{
			foreach (var task in tasks)
			{
				tasksInfo.AppendLine($"{i++}) {buildRow(task)}\n");
			}
		}
		else
		{
			tasksInfo.AppendLine(_localizer["no_tasks"]);
		}

		return tasksInfo;
	}

	private static string GetFormattedDateTime(DateTime dateTime)
	{
		return dateTime
			.ToString(dateTime.TimeOfDay != TimeSpan.Zero 
				? "dd.MM.yyyy HH:mm"
				: "dd.MM.yyyy");
	}
}