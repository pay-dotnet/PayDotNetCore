using System.Reflection;

namespace PayDotNet.Core.Tests;

public class TestBase<TSystemUnderTest>
{
    public MockRepository MockRepository = new(MockBehavior.Strict);
    private readonly Dictionary<Type, Mock> _mockDictionary = new Dictionary<Type, Mock>();
    private TSystemUnderTest? _systemUnderTest;

    protected TestBase()
    {
        _systemUnderTest = default;
    }

    public TSystemUnderTest SystemUnderTest => GetSystemUnderTest();

    protected virtual TSystemUnderTest CreateSystemUnderTest()
    {
        Type type = typeof(TSystemUnderTest);
        if (!type.IsClass || type.IsAbstract)
        {
            throw new InvalidOperationException("Invalid type for SUT.");
        }

        ConstructorInfo? constructor = type
            .GetConstructors(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Instance)
            .OrderBy(x => x.GetParameters().Count())
            .FirstOrDefault();
        if (constructor == null)
        {
            string message = string.Format("SUT has no constructor", type.Name);
            throw new InvalidOperationException(message);
        }

        object?[] parameters = GetParameters(constructor.GetParameters());
        object result = constructor.Invoke(parameters);
        return (TSystemUnderTest)result;
    }

    protected object?[] GetParameters(ParameterInfo[] parameters)
    {
        List<object?> result = new List<object?>();

        foreach (ParameterInfo parameter in parameters)
        {
            Type type = parameter.ParameterType;
            if (_mockDictionary.ContainsKey(type))
            {
                result.Add(_mockDictionary[type].Object);
            }
            else
            {
                result.Add(null);
            }
        }

        return result.ToArray();
    }

    protected Mock<T> Mocks<T>()
        where T : class
    {
        Type type = typeof(T);
        if (!_mockDictionary.ContainsKey(type))
        {
            Mock<T> mock = MockRepository.Create<T>();
            _mockDictionary.Add(type, mock);
        }

        return (Mock<T>)_mockDictionary[type];
    }

    private TSystemUnderTest GetSystemUnderTest()
    {
        if (_systemUnderTest == null)
        {
            _systemUnderTest = CreateSystemUnderTest();
        }

        return _systemUnderTest;
    }
}