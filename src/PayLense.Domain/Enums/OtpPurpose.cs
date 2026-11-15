namespace PayLense.Domain.Enums;

public enum OtpPurpose
{
    EmailConfirmation,
    PhoneConfirmation,
    LoginTwoFactor
}

public enum ContactType
{
    Phone,
    Email
}

public enum ChangeRequestStatus
{
    Pending,
    Completed,
    Rejected,
    Expired
}

public enum TransactionSendType
{
    Instant,
    Delayed,
    Secure,
    Recurring
}

public enum TransactionSource
{
    Direct,
    Request,
    Bill,
    Automated
}

public enum TransactionStatus
{
    Pending,
    Processing,
    Completed,
    Failed,
    Cancelled,
    Reversed
}

public enum LedgerEntryType
{
    Debit,
    Credit
}

public enum MoneyRequestStatus
{
    Pending,
    Accepted,
    Rejected,
    Cancelled,
    Expired
}

public enum LimitPeriodType
{
    Daily,
    Weekly,
    Monthly
}

public enum AutomatedSendFrequency
{
    Daily,
    Weekly,
    Monthly
}

public enum ChatSenderType
{
    User,
    Bot
}
