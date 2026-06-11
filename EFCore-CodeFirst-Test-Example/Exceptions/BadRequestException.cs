namespace EFCore_CodeFirst_Test_Example.Exceptions;

public class BadRequestException(string msg) : Exception(msg);
