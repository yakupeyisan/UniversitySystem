using FluentValidation;
using Academic.Application.DTOs;

namespace Academic.Application.Validators;

public class CreateCourseRequestValidator : AbstractValidator<CreateCourseRequest>
{
    public CreateCourseRequestValidator()
    {
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Kurs kodu boþ olamaz")
            .Length(4, 20).WithMessage("Kurs kodu 4-20 karakter arasýnda olmalýdýr")
            .Matches(@"^[A-Z]{2,4}\d{2,4}$").WithMessage("Kurs kodu format hatasý (örn: CS101)");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs adý boþ olamaz")
            .Length(3, 200).WithMessage("Kurs adý 3-200 karakter arasýnda olmalýdýr");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açýklama maksimum 1000 karakter olabilir");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");

        RuleFor(x => x.Level)
            .InclusiveBetween(1, 4).WithMessage("Seviye 1-4 arasýnda olmalýdýr");

        RuleFor(x => x.Type)
            .InclusiveBetween(1, 5).WithMessage("Tür 1-5 arasýnda olmalýdýr");

        RuleFor(x => x.Semester)
            .InclusiveBetween(1, 3).WithMessage("Semester 1-3 arasýnda olmalýdýr");

        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Yýl 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(4).WithMessage("Yýl maksimum 4 olabilir");

        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Bölüm ID'si boþ olamaz");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}

public class UpdateCourseRequestValidator : AbstractValidator<UpdateCourseRequest>
{
    public UpdateCourseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Kurs adý boþ olamaz")
            .Length(3, 200).WithMessage("Kurs adý 3-200 karakter arasýnda olmalýdýr");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Açýklama maksimum 1000 karakter olabilir");

        RuleFor(x => x.ECTS)
            .GreaterThan(0).WithMessage("ECTS 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(20).WithMessage("ECTS maksimum 20 olabilir");

        RuleFor(x => x.Credits)
            .GreaterThan(0).WithMessage("Kredi 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(10).WithMessage("Kredi maksimum 10 olabilir");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");
    }
}

public class CancelCourseRequestValidator : AbstractValidator<CancelCourseRequest>
{
    public CancelCourseRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýptal nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ýptal nedeni maksimum 500 karakter olabilir");
    }
}

public class AddInstructorRequestValidator : AbstractValidator<AddInstructorRequest>
{
    public AddInstructorRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Öðretmen ID'si boþ olamaz");
    }
}

public class AddPrerequisiteRequestValidator : AbstractValidator<AddPrerequisiteRequest>
{
    public AddPrerequisiteRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.PrerequisiteCourseId)
            .NotEmpty().WithMessage("Ön koþul kurs ID'si boþ olamaz");

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (request.CourseId == request.PrerequisiteCourseId)
                    context.AddFailure("Bir kurs kendisinin ön koþulu olamaz");
            });
    }
}

public class RecordGradeRequestValidator : AbstractValidator<RecordGradeRequest>
{
    public RecordGradeRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.RegistrationId)
            .NotEmpty().WithMessage("Kayýt ID'si boþ olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester boþ olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara sýnav puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara sýnav puaný 100'den büyük olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puaný 100'den büyük olamaz");

        RuleFor(x => x.ECTS)
            .GreaterThanOrEqualTo(0).WithMessage("ECTS 0'dan küçük olamaz")
            .LessThanOrEqualTo(20).WithMessage("ECTS 20'den büyük olamaz");
    }
}

public class UpdateGradeRequestValidator : AbstractValidator<UpdateGradeRequest>
{
    public UpdateGradeRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si boþ olamaz");

        RuleFor(x => x.MidtermScore)
            .GreaterThanOrEqualTo(0).WithMessage("Ara sýnav puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Ara sýnav puaný 100'den büyük olamaz");

        RuleFor(x => x.FinalScore)
            .GreaterThanOrEqualTo(0).WithMessage("Final puaný 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Final puaný 100'den büyük olamaz");
    }
}

public class SubmitGradeObjectionRequestValidator : AbstractValidator<SubmitGradeObjectionRequest>
{
    public SubmitGradeObjectionRequestValidator()
    {
        RuleFor(x => x.GradeId)
            .NotEmpty().WithMessage("Not ID'si boþ olamaz");

        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýtirazi neden boþ olamaz")
            .MaximumLength(500).WithMessage("Ýtirazi neden maksimum 500 karakter olabilir");
    }
}

public class ApproveGradeObjectionRequestValidator : AbstractValidator<ApproveGradeObjectionRequest>
{
    public ApproveGradeObjectionRequestValidator()
    {
        RuleFor(x => x.ObjectionId)
            .NotEmpty().WithMessage("Ýtiraz ID'si boþ olamaz");

        RuleFor(x => x.ReviewedBy)
            .NotEmpty().WithMessage("Ýnceleyici ID'si boþ olamaz");

        RuleFor(x => x.ReviewNotes)
            .MaximumLength(1000).WithMessage("Ýnceleme notlarý maksimum 1000 karakter olabilir");

        RuleFor(x => x.NewScore)
            .GreaterThanOrEqualTo(0).WithMessage("Yeni puan 0'dan küçük olamaz")
            .LessThanOrEqualTo(100).WithMessage("Yeni puan 100'den büyük olamaz")
            .When(x => x.NewScore.HasValue);
    }
}

public class EnrollStudentRequestValidator : AbstractValidator<EnrollStudentRequest>
{
    public EnrollStudentRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester boþ olamaz");
    }
}

public class DropCourseRequestValidator : AbstractValidator<DropCourseRequest>
{
    public DropCourseRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ders býrakma nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ders býrakma nedeni maksimum 500 karakter olabilir");
    }
}

public class JoinWaitingListRequestValidator : AbstractValidator<JoinWaitingListRequest>
{
    public JoinWaitingListRequestValidator()
    {
        RuleFor(x => x.StudentId)
            .NotEmpty().WithMessage("Öðrenci ID'si boþ olamaz");

        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.Semester)
            .NotEmpty().WithMessage("Semester boþ olamaz");
    }
}


public class ScheduleExamRequestValidator : AbstractValidator<ScheduleExamRequest>
{
    public ScheduleExamRequestValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty().WithMessage("Kurs ID'si boþ olamaz");

        RuleFor(x => x.ExamType)
            .InclusiveBetween(1, 5).WithMessage("Sýnav türü 1-5 arasýnda olmalýdýr");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("Sýnav tarihi boþ olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Sýnav tarihi format hatasý (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Sýnav tarihi geçmiþ tarih olamaz");
                }
                else
                    context.AddFailure("Sýnav tarihi geçersiz");
            });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Baþlangýç saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Baþlangýç saati format hatasý (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Baþlangýç saati geçersiz");
            });

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Bitiþ saati format hatasý (HH:mm)")
            .Custom((time, context) =>
            {
                if (!TimeOnly.TryParse(time, out _))
                    context.AddFailure("Bitiþ saati geçersiz");
            });

        RuleFor(x => x)
            .Custom((request, context) =>
            {
                if (TimeOnly.TryParse(request.StartTime, out var start) &&
                    TimeOnly.TryParse(request.EndTime, out var end))
                {
                    if (end <= start)
                        context.AddFailure("Bitiþ saati baþlangýç saatinden sonra olmalýdýr");
                }
            });

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(0).WithMessage("Maksimum kapasite 0'dan büyük olmalýdýr")
            .LessThanOrEqualTo(500).WithMessage("Maksimum kapasite 500 olamaz");

        RuleFor(x => x.IsOnline)
            .Custom((isOnline, context) =>
            {
                var request = context.InstanceToValidate;
                if (isOnline && string.IsNullOrWhiteSpace(request.OnlineLink))
                    context.AddFailure("Online sýnav için link saðlanmalýdýr");
            });

        RuleFor(x => x.OnlineLink)
            .Must(url => string.IsNullOrWhiteSpace(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Online link geçerli bir URL olmalýdýr")
            .When(x => !string.IsNullOrWhiteSpace(x.OnlineLink));
    }
}

public class UpdateExamRequestValidator : AbstractValidator<UpdateExamRequest>
{
    public UpdateExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.ExamDate)
            .NotEmpty().WithMessage("Sýnav tarihi boþ olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Sýnav tarihi format hatasý (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Sýnav tarihi geçmiþ tarih olamaz");
                }
                else
                    context.AddFailure("Sýnav tarihi geçersiz");
            });

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Baþlangýç saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Baþlangýç saati format hatasý (HH:mm)");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Bitiþ saati format hatasý (HH:mm)");
    }
}

public class PostponeExamRequestValidator : AbstractValidator<PostponeExamRequest>
{
    public PostponeExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.NewExamDate)
            .NotEmpty().WithMessage("Yeni sýnav tarihi boþ olamaz")
            .Matches(@"^\d{4}-\d{2}-\d{2}$").WithMessage("Yeni sýnav tarihi format hatasý (yyyy-MM-dd)")
            .Custom((date, context) =>
            {
                if (DateOnly.TryParse(date, out var parsedDate))
                {
                    if (parsedDate < DateOnly.FromDateTime(DateTime.UtcNow))
                        context.AddFailure("Yeni sýnav tarihi geçmiþ tarih olamaz");
                }
                else
                    context.AddFailure("Yeni sýnav tarihi geçersiz");
            });

        RuleFor(x => x.NewStartTime)
            .NotEmpty().WithMessage("Yeni baþlangýç saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni baþlangýç saati format hatasý (HH:mm)");

        RuleFor(x => x.NewEndTime)
            .NotEmpty().WithMessage("Yeni bitiþ saati boþ olamaz")
            .Matches(@"^\d{2}:\d{2}$").WithMessage("Yeni bitiþ saati format hatasý (HH:mm)");
    }
}

public class CancelExamRequestValidator : AbstractValidator<CancelExamRequest>
{
    public CancelExamRequestValidator()
    {
        RuleFor(x => x.ExamId)
            .NotEmpty().WithMessage("Sýnav ID'si boþ olamaz");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Ýptal nedeni boþ olamaz")
            .MaximumLength(500).WithMessage("Ýptal nedeni maksimum 500 karakter olabilir");
    }
}
