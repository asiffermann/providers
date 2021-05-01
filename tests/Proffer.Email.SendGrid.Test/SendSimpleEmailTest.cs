namespace Proffer.Email.SendGrid.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Proffer.Email.Internal;
    using Proffer.Testing;
    using Xunit;
    using Xunit.Categories;

    [IntegrationTest]
    [Feature(nameof(Email))]
    [Feature(nameof(SendGrid))]
    [Collection(nameof(SendGridCollection))]
    public class SendSimpleEmailTest
    {
        private readonly SendGridFixture storeFixture;
        private readonly IEmailSender emailSender;

        public SendSimpleEmailTest(SendGridFixture fixture)
        {
            this.storeFixture = fixture;
            this.emailSender = this.storeFixture.Services.GetRequiredService<IEmailSender>();
        }

        [Fact]
        public async Task Send()
        {
            await this.emailSender.SendEmailAsync("Simple mail", "Hello, it's a simple mail", new EmailAddress
            {
                DisplayName = "test user",
                Email = "tests@proffer-dotnet.org"
            });
        }

        [Fact]
        public async Task SendWithReplyTo()
        {
            await this.emailSender.SendEmailAsync(
                this.storeFixture.DefaultSender,
                new EmailAddress
                {
                    DisplayName = "Reply Address",
                    Email = "tests@proffer-dotnet.org"
                },
                "Simple mail", "Hello, it's a simple mail", false,
                new EmailAddress
                {
                    DisplayName = "test user",
                    Email = "hello@proffer-dotnet.org"
                });
        }

        [Fact]
        public async Task SendWithCC()
        {
            await this.emailSender.SendEmailAsync(
                this.storeFixture.DefaultSender,
                "Cc test",
                "Hello, this is an email with cc recipients",
                Enumerable.Empty<IEmailAttachment>(),
                new EmailAddress
                {
                    DisplayName = "recipient user",
                    Email = SendGridFixture.FirstRecipient
                }.Yield(),
                new EmailAddress
                {
                    DisplayName = "cc user",
                    Email = SendGridFixture.SecondRecipient
                }.Yield(),
                Array.Empty<IEmailAddress>());
        }

        [Fact]
        public async Task SendWithBcc()
        {
            await this.emailSender.SendEmailAsync(
                this.storeFixture.DefaultSender,
                "Bcc test",
                "Hello, this is an email with bcc recipients",
                Enumerable.Empty<IEmailAttachment>(),
                new EmailAddress
                {
                    DisplayName = "recipient user",
                    Email = SendGridFixture.FirstRecipient
                }.Yield(),
                Array.Empty<IEmailAddress>(),
                new EmailAddress
                {
                    DisplayName = "test user",
                    Email = SendGridFixture.SecondRecipient
                }.Yield());
        }

        [Fact]
        public async Task SendWithAttachments()
        {
            byte[] data = System.IO.File.ReadAllBytes(@"Attachments/beach.jpeg");
            var image = new EmailAttachment("Beach.jpeg", data, "image", "jpeg");

            data = System.IO.File.ReadAllBytes(@"Attachments/sample.pdf");
            var pdf = new EmailAttachment("Sample.pdf", data, "application", "pdf");

            await this.emailSender.SendEmailAsync(
                this.storeFixture.DefaultSender,
                "Test mail with attachments",
                "Hello, this is an email with attachments",
                new List<IEmailAttachment> { image, pdf },
                new EmailAddress
                {
                    DisplayName = "test user",
                    Email = SendGridFixture.FirstRecipient
                });
        }

        [Fact]
        public async Task ErrorSendWithCCDuplicates()
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                 this.emailSender.SendEmailAsync(
                     this.storeFixture.DefaultSender,
                     "Cc test",
                     "Hello, this is an email with cc recipients",
                     Enumerable.Empty<IEmailAttachment>(),
                     new EmailAddress
                     {
                         DisplayName = "recipient user",
                         Email = SendGridFixture.SecondRecipient
                     }.Yield(),
                     new EmailAddress
                     {
                         DisplayName = "cc user",
                         Email = SendGridFixture.SecondRecipient
                     }.Yield(),
                     Array.Empty<IEmailAddress>()));
        }
    }
}
