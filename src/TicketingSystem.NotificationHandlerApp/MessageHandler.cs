﻿using com.ticketingSystem;
using Polly;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TicketingSystem.BusinessLogic.Services;
using TicketingSystem.Common.Enums;
using TicketingSystem.Messaging;
using TicketingSystem.NotificationHandlerApp.EmailProviders;
using TicketingSystem.NotificationHandlerApp.Models;

namespace TicketingSystem.NotificationHandlerApp
{
    public class MessageHandler(IEmailProvider emailProvider, INotificationService notificationService) : IMessageHandler
    {
        private readonly INotificationService _notificationService = notificationService;
        private readonly IEmailProvider _emailProvider = emailProvider;

        private readonly Dictionary<OperationType, string> Subjects = new()
        {
            [OperationType.SeatsBooked] = "Seats booking notification",
            [OperationType.PaymentSucceeded] = "Successful payment",
        };

        public async Task HandleAsync(MessageValue message, CancellationToken ct = default)
        {
            var policy = Policy.Handle<HttpRequestException>()
                               .OrResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                               .RetryAsync(3, onRetry: (exception, attemptNumber) =>
                               {
                                   Console.WriteLine($"Attempt {attemptNumber} of sending an email.");
                               });

            await policy.ExecuteAsync(async () => await CreateAndSendEmail(message, ct));
        }

        public async Task<HttpResponseMessage> CreateAndSendEmail(MessageValue message, CancellationToken ct = default)
        {
            var emailRequest = new EmailModel
            {
                FromEmail = "donotreply@ticketingsyst.em",
                FromName = "Ticketing System Auto Notification",
                Subject = Subjects[(OperationType)message.Operation],
                TextPart = message.OrderSummary,
                Recipients = [new EmailRecipientModel { Email = message.CustomerEmail }]
            };

            var response = await _emailProvider.SendEmailAsync(emailRequest, ct);

            NotificationStatus notificationStatus = response.IsSuccessStatusCode ?
                NotificationStatus.Success
                : NotificationStatus.Failed;

            var outputMessage = response.IsSuccessStatusCode ?
                $"Notification {message.TrackingId} was sent successfully"
                : $"An error occured while sending the Notification {message.TrackingId} was sent successfully";

            Console.WriteLine(outputMessage);

            await _notificationService.UpdateNotificationStatus(message.TrackingId, notificationStatus);

            return response;
        }
    }
}