using Microsoft.Extensions.Options;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using System.Net;
using System.Net.Mail;

namespace Shop.Infrastructure.Services;

public class EmailService(
    IOptions<EmailSettings> options) : IEmailService
{
    private const int TokenLifetimeMinutes = 30;

    private readonly EmailSettings _settings = options.Value;

    public async Task SendPasswordResetEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var link = BuildPasswordLink(token);

        var body = BuildEmailBody(
            title: "Відновлення пароля",
            message:
                """
                Ви отримали цей лист, тому що було запитано
                відновлення пароля для вашого облікового запису Shop.
                """,
            actionText: "Відновити пароль",
            link: link,
            additionalMessage:
                """
                Якщо ви не запитували відновлення пароля,
                просто проігноруйте цей лист.
                """
        );

        await SendEmailAsync(
            email,
            "Відновлення пароля - Shop",
            body,
            cancellationToken);
    }

    public async Task SendPasswordSetupEmailAsync(
        string email,
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        var link = BuildPasswordLink(token);

        var body = BuildEmailBody(
            title: "Вас додали до адміністративної панелі Shop",
            message:
                """
                Для вашої електронної адреси було створено
                обліковий запис у адміністративній панелі Shop.
                """,
            actionText: "Встановити пароль",
            link: link,
            additionalMessage:
                """
                Перейдіть за посиланням, щоб встановити пароль
                для вашого облікового запису.
                """
        );

        await SendEmailAsync(
            email,
            "Встановлення пароля — Shop",
            body,
            cancellationToken);
    }

    private string BuildPasswordLink(string token)
    {
        var frontendUrl =
            _settings.FrontendUrl.TrimEnd('/');

        return
            $"{frontendUrl}/reset-password" +
            $"?token={Uri.EscapeDataString(token)}";
    }

    private static string BuildEmailBody(
        string title,
        string message,
        string actionText,
        string link,
        string additionalMessage)
    {
        var encodedTitle =
            WebUtility.HtmlEncode(title);

        var encodedMessage =
            WebUtility.HtmlEncode(message)
                .Replace(Environment.NewLine, "<br>");

        var encodedActionText =
            WebUtility.HtmlEncode(actionText);

        var encodedLink =
            WebUtility.HtmlEncode(link);

        var encodedAdditionalMessage =
            WebUtility.HtmlEncode(additionalMessage)
                .Replace(Environment.NewLine, "<br>");

        return $"""
            <!DOCTYPE html>
            <html lang="uk">
            <head>
                <meta charset="UTF-8">
                <meta name="viewport"
                      content="width=device-width, initial-scale=1.0">
                <title>{encodedTitle}</title>
            </head>

            <body style="
                margin: 0;
                padding: 0;
                background-color: #f5f5f5;
                font-family: Arial, sans-serif;
                color: #222222;
            ">
                <div style="
                    max-width: 600px;
                    margin: 40px auto;
                    background: #ffffff;
                    padding: 32px;
                    border-radius: 8px;
                ">

                    <h2 style="
                        margin-top: 0;
                        margin-bottom: 20px;
                    ">
                        {encodedTitle}
                    </h2>

                    <p>
                        {encodedMessage}
                    </p>

                    <p style="margin: 30px 0;">
                        <a href="{encodedLink}"
                           style="
                               display: inline-block;
                               padding: 12px 22px;
                               background-color: #2563eb;
                               color: #ffffff;
                               text-decoration: none;
                               border-radius: 6px;
                           ">
                            {encodedActionText}
                        </a>
                    </p>

                    <p>
                        Посилання дійсне протягом
                        <strong>
                            {TokenLifetimeMinutes} хвилин
                        </strong>.
                    </p>

                    <p>
                        {encodedAdditionalMessage}
                    </p>

                    <hr style="
                        margin: 30px 0;
                        border: 0;
                        border-top: 1px solid #eeeeee;
                    ">

                    <p style="
                        font-size: 12px;
                        color: #777777;
                    ">
                        Якщо кнопка не працює, скопіюйте це посилання
                        у браузер:
                    </p>

                    <p style="
                        font-size: 12px;
                        word-break: break-all;
                    ">
                        <a href="{encodedLink}">
                            {encodedLink}
                        </a>
                    </p>

                    <p style="
                        margin-top: 30px;
                        font-size: 12px;
                        color: #999999;
                    ">
                        Це автоматичний лист. Будь ласка, не відповідайте
                        на нього.
                    </p>

                </div>
            </body>
            </html>
            """;
    }

    private async Task SendEmailAsync(
        string email,
        string subject,
        string body,
        CancellationToken cancellationToken)
    {
        using var message = new MailMessage
        {
            From = new MailAddress(
                _settings.From,
                _settings.FromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = true
        };

        message.To.Add(email);

        using var client = new SmtpClient(
            _settings.Host,
            _settings.Port)
        {
            EnableSsl = _settings.EnableSsl,
            Credentials = new NetworkCredential(
                _settings.Username,
                _settings.Password)
        };

        cancellationToken.ThrowIfCancellationRequested();

        await client.SendMailAsync(message);
    }

    public async Task SendOrderConfirmationEmailAsync(string email, IReadOnlyList<OrderProcessingProductDTO> products, decimal totalPrice, int orderId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var rows = string.Join(
            "",
            products.Select(product =>
            {
                var itemTotal = product.Price * product.Count;

                return $"""
                <tr>
                    <td style="padding: 10px; border-bottom: 1px solid #eeeeee;">
                        {WebUtility.HtmlEncode(product.ProductName)}
                    </td>
                    <td style="padding: 10px; border-bottom: 1px solid #eeeeee;">
                        {product.Price:F2}
                    </td>
                    <td style="padding: 10px; border-bottom: 1px solid #eeeeee;">
                        {product.Count}
                    </td>
                    <td style="padding: 10px; border-bottom: 1px solid #eeeeee;">
                        {itemTotal:F2}
                    </td>
                </tr>
                """;
            }));

        var body = $"""
        <!DOCTYPE html>
        <html lang="uk">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Замовлення #{orderId}</title>
        </head>

        <body style="
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            font-family: Arial, sans-serif;
            color: #222222;
        ">
            <div style="
                max-width: 700px;
                margin: 40px auto;
                background: #ffffff;
                padding: 32px;
                border-radius: 8px;
            ">

                <h2>
                    Дякуємо за ваше замовлення!
                </h2>

                <p>
                    Замовлення
                    <strong>#{orderId}</strong>
                    успішно створено.
                </p>

                <table style="width: 100%; border-collapse: collapse; margin-top: 25px;
                ">
                    <thead>
                        <tr>
                            <th style="text-align: left; padding: 10px; border-bottom: 2px solid #dddddd;
                            ">
                                Товар
                            </th>

                            <th style="text-align: left; padding: 10px; border-bottom: 2px solid #dddddd;
                            ">
                                Ціна
                            </th>

                            <th style="text-align: left; padding: 10px; border-bottom: 2px solid #dddddd;
                            ">
                                Кількість
                            </th>

                            <th style="text-align: left; padding: 10px; border-bottom: 2px solid #dddddd;
                            ">
                                Сума
                            </th>
                        </tr>
                    </thead>

                    <tbody>
                        {rows}
                    </tbody>
                </table>

                <div style="margin-top: 25px; text-align: right; font-size: 18px;
                ">
                    <strong>
                        Загальна сума: {totalPrice:F2}
                    </strong>
                </div>

                <p style="margin-top: 30px; color: #777777;
                ">
                    Це автоматичний лист. Будь ласка, не відповідайте на нього.
                </p>

            </div>
        </body>
        </html>
        """;

        await SendEmailAsync(email, $"Замовлення #{orderId} - Shop", body, cancellationToken);
    }

    public async Task SendOrderWaitingEmailAsync(string email, IReadOnlyList<OrderProcessingProductDTO> unavailableProducts, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(email);

        var rows = string.Join(
            "",
            unavailableProducts.Select(product =>
            {
                return $"""
                <li style="margin-bottom: 8px;">
                    <strong>
                        {WebUtility.HtmlEncode(product.ProductName)}
                    </strong>
                    - кількість: {product.Count}
                </li>
                """;
            }));

        var body = $"""
        <!DOCTYPE html>
        <html lang="uk">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Замовлення очікує</title>
        </head>

        <body style="
            margin: 0;
            padding: 0;
            background-color: #f5f5f5;
            font-family: Arial, sans-serif;
            color: #222222;
        ">
            <div style="
                max-width: 600px;
                margin: 40px auto;
                background: #ffffff;
                padding: 32px;
                border-radius: 8px;
            ">

                <h2>
                    Замовлення очікує
                </h2>

                <p>
                    На жаль, зараз на складі недостатньо деяких товарів для виконання вашого замовлення.
                </p>

                <p>
                    Товари, яких недостатньо:
                </p>

                <ul>
                    {rows}
                </ul>

                <p style="margin-top: 25px; color: #555555;
                ">
                    Замовлення не було створено та товар зі складу не списувався.
                </p>

                <p style="margin-top: 30px; font-size: 12px; color: #999999;
                ">
                    Це автоматичний лист. Будь ласка, не відповідайте на нього.
                </p>

            </div>
        </body>
        </html>
        """;

        await SendEmailAsync(email, "Замовлення очікує - Shop", body, cancellationToken);
    }

}