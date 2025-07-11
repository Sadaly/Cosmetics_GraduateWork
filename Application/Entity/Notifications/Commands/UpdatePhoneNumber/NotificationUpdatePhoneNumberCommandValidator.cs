﻿using FluentValidation;
using Domain.ValueObjects;

namespace Application.Entity.Notifications.Commands.UpdatePhoneNumber
{
    public class NotificationUpdatePhoneNumberCommandValidator : AbstractValidator<NotificationUpdatePhoneNumberCommand>
    {
        public NotificationUpdatePhoneNumberCommandValidator()
        {
            RuleFor(x => x.PhoneNumber)
                .NotNull()
                .MinimumLength(PhoneNumber.MIN_LENGTH)
                .MinimumLength(PhoneNumber.MIN_LENGTH);
        }
    }
}
