using Microsoft.Extensions.Options;
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
}