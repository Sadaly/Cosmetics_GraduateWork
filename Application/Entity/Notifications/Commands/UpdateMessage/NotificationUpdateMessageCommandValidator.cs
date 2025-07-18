﻿using FluentValidation;
using Domain.ValueObjects;

namespace Application.Entity.Notifications.Commands.UpdateMessage
{
    public class NotificationUpdateMessageCommandValidator : AbstractValidator<NotificationUpdateMessageCommand>
    {
        public NotificationUpdateMessageCommandValidator()
        {
            RuleFor(x => x.Message)
                .NotNull()
                .MinimumLength(PhoneNumber.MIN_LENGTH)
                .MinimumLength(PhoneNumber.MIN_LENGTH);
        }
    }
}
