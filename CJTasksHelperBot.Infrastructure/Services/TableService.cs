using System.Text;
using Alba.CsConsoleFormat;
using CJTasksHelperBot.Application.Common.Models;
using CJTasksHelperBot.Infrastructure.Common.Interfaces.Services;
using CJTasksHelperBot.Infrastructure.Resources;
using Microsoft.Extensions.Localization;

namespace CJTasksHelperBot.Infrastructure.Services;

public class TableService : ITableService
{
	private static readonly LineThickness StrokeHeader = new LineThickness(LineWidth.None, LineWidth.Double);
	private static readonly LineThickness StrokeRight = new LineThickness(LineWidth.None, LineWidth.None, LineWidth.Single, LineWidth.None);

	private readonly IStringLocalizer<Messages> _localizer;

	public TableService(IStringLocalizer<Messages> localizer)
	{
		_localizer = localizer;
	}

	public string GetTable(IEnumerable<GetTaskDto> items)
	{
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
					items.Select(item => new[]
					{
						new Cell(i++) {Stroke = StrokeRight},
						new Cell(item.Title) {Stroke = StrokeRight},
						new Cell(item.Deadline.ToString("dd.MM.yyyy HH:mm")) {Stroke = StrokeRight, Align = Align.Center},
						new Cell(item.Description) {Stroke = LineThickness.None},
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
}