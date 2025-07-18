﻿using Domain.Common;
using Domain.Errors;
using Domain.Shared;
using Domain.ValueObjects;
using System.Text.Json.Serialization;

namespace Domain.Entity
{
    public class Notification : BaseEntity
    {
        private Notification(Guid id) : base(id) { }
        private Notification(Guid id, Procedure procedure, Text message, PhoneNumber phoneNumber, DateTime sendingDate) : base(id)
        {
            Procedure = procedure;
            ProcedureId = Procedure.Id;
            Message = message;
            SendingDate = sendingDate;
            IsSent = false;
            PhoneNumber = phoneNumber;
        }

        [JsonIgnore]
        public Procedure Procedure { get; set; } = null!;
        public PhoneNumber PhoneNumber { get; set; } = null!;
        public Guid ProcedureId { get; set; }
        public Text Message { get; set; } = null!;
        public DateTime SendingDate { get; set; }
        public bool IsSent { get; set; }

        public static Result<Notification> Create(Result<Procedure> procedure, Result<Text> message, DateTime sendingDate, Result<PhoneNumber> phoneNumber)
        {
            if (procedure.IsFailure) return Result.Failure<Notification>(procedure);
            if (phoneNumber.IsFailure) return Result.Failure<Notification>(phoneNumber);
            if (message.IsFailure) return Result.Failure<Notification>(message);
            if (procedure.Value.ScheduledDate == null) return Result.Failure<Notification>(DomainErrors.Notification.ProcedureNotScheduled);
            if (sendingDate > procedure.Value.ScheduledDate) return Result.Failure<Notification>(DomainErrors.Notification.Late);

            return new Notification(Guid.NewGuid(), procedure.Value, message.Value, phoneNumber.Value, sendingDate);
        }

        public Result<Notification> UpdateMessage(Result<Text> message)
        {
            if (message.IsFailure) return Result.Failure<Notification>(message.Error);
            this.Message = message.Value;
            return this;
        }

        public Result<Notification> UpdatePhoneNumber(Result<PhoneNumber> phoneNumber)
        {
            if (phoneNumber.IsFailure) return Result.Failure<Notification>(phoneNumber.Error);
            this.PhoneNumber = phoneNumber.Value;
            return this;
        }
    }
}
