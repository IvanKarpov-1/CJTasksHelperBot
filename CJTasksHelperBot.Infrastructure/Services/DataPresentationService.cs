using System.Text;
using Alba.CsConsoleFormat;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;

namespace CJTasksHelperBot.Infrastructure.Services;

public class DataPresentationService : IDataPresentationService
{
	private static readonly LineThickness StrokeHeader = new(LineWidth.None, LineWidth.Double);
	private static readonly LineThickness StrokeRight = new(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);

	private readonly IStringLocalizer<Messages> _localizer;

	public DataPresentationService(IStringLocalizer<Messages> localizer)
	{
		_localizer = localizer;
	}

	public string GetTabledTextRepresentation(IEnumerable<GetTaskDto> items)
	{
		var tasks = items.ToList();
		
		var i = 1;

		var document = new Document(
			new Grid
			{
				Columns =
				{
					GridLength.Auto,
					GridLength.Char(15),
					GridLength.Char(16),
					GridLength.Char(25)
				},
				Stroke = StrokeHeader,
				Children =
				{
					new Cell("№") {Stroke = StrokeHeader},
					new Cell(_localizer["table_cell_name"]) {Stroke = StrokeHeader},
					new Cell(_localizer["table_cell_deadline"]) {Stroke = StrokeHeader},
					new Cell(_localizer["table_cell_description"]) {Stroke = StrokeHeader},
					tasks.Select(task => new[]
					{
						new Cell(i++) {Stroke = StrokeRight},
						new Cell(task.Title) {Stroke = StrokeRight},
						new Cell(task.Deadline.ToString("dd.MM.yyyy HH:mm")) {Stroke = StrokeRight, Align = Align.Center},
						new Cell(task.Description) {Stroke = LineThickness.None},
					})
				}
			});

		var textRenderTarget = new TextRenderTarget();
		ConsoleRenderer.RenderDocumentToText(document, textRenderTarget);

		using var sr = new StringReader(textRenderTarget.OutputText);

		var table = new StringBuilder();

		while (sr.ReadLine() is { } line)
		{
			table.AppendLine(line.TrimEnd());
		}

		return table.ToString();
	}

	public string GetPlainTextRepresentation(IEnumerable<GetTaskDto> items)
	{
		var tasks = items.ToList();
		
		var tasksInfo = new StringBuilder();
		var i = 1;

		if (tasks.Count != 0)
		{
			foreach (var task in tasks)
			{
				tasksInfo.AppendLine(
					$"{i++}) {task.Title} |" +
					$" {_localizer["word_by"]}: {task.Deadline} |" +
					$" {task.Description} |" /* +
					$" {_localizer["word_status"]}: {TaskStatusCustomEnum.FromValue((int)task.Status).DisplayName};\n" */);
			}
		}
		else
		{
			tasksInfo.AppendLine(_localizer["no_tasks"]);
		}

		return tasksInfo.ToString();
	}
}