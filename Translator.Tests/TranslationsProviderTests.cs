using FluentAssertions;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UtilsPackages.Common.Cache;

namespace Translator.Tests
{
    public class TranslationsProviderTests
    {
        private Dictionary<string, Mock<ITranslationsReader>>? _readers;
        private Mock<ICache<TranslationKey>>? _cache;
        private TranslationsProviderOptions? _options;

        public IEnumerable<KeyValuePair<string, string>>? ThenReadTranslations;

        [SetUp]
        public void Setup()
        {
            _readers = new Dictionary<string, Mock<ITranslationsReader>>();
            _cache = new Mock<ICache<TranslationKey>>();
            _options = new TranslationsProviderOptions
            {
                Cache = CacheOptions.NoCache(),
            };
        }

        [Test]
        public void ReadTranslations_ShouldReadFromAllReaders()
        {
            GivenTranslationReaderReturns("reader", new TranslationsConfig {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello" }
                }
            });
            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "World", "World" }
                }
            });

            WhenReadTranslationsIsCalled("en");

            ThenAllReadersShouldBeReadTimes(1);
            ThenReadTranslations.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "Hello", "Hello" },
                { "World", "World" }
            });
        }

        [Test]
        public void ReadTranslations_ShouldReadOnlyProvidedLanguage()
        {
            GivenTranslationReaderReturns("reader", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello1" }
                }
            });
            GivenTranslationReaderReturns("reader", new TranslationsConfig
            {
                Language = "us",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello2" }
                }
            });

            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "World", "World1" }
                }
            });
            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "us",
                Translations = new Dictionary<string, string>
                {
                    { "World", "World2" }
                }
            });

            WhenReadTranslationsIsCalled("en");

            ThenAllReadersShouldBeReadTimes(1);
            ThenReadTranslations.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "Hello", "Hello1" },
                { "World", "World1" }
            });
        }

        [Test]
        public void ReadTranslations_ShouldNotReturnDuplicatedTranslationKeys()
        {
            GivenTranslationReaderReturns("reader", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello1" }
                }
            });
            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello2" }
                }
            });

            WhenReadTranslationsIsCalled("en");

            ThenAllReadersShouldBeReadTimes(1);
            ThenReadTranslations.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "Hello", "Hello1" }
            });
        }

        [Test]
        public void ReadTranslations_ShouldThrowExceptionIfNoReaderKnowsTheLanguage()
        {
            GivenTranslationReaderReturns("reader", new TranslationsConfig
            {
                Language = "anotherLanguage",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello1" }
                }
            });
            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "anotherLanguage",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello2" }
                }
            });

            var action = () => WhenReadTranslationsIsCalled("en");

            action.Should().Throw<LanguageNotConfiguredException>();
        }

        [Test]
        public void ReadTranslations_ShouldNotThrowExceptionIfAnyReaderKnowsLanguage()
        {
            GivenTranslationReaderThrows("reader", new LanguageNotConfiguredException("en"));
            GivenTranslationReaderReturns("reader2", new TranslationsConfig
            {
                Language = "en",
                Translations = new Dictionary<string, string>
                {
                    { "Hello", "Hello1" }
                }
            });

            var action = () => WhenReadTranslationsIsCalled("en");

            action.Should().NotThrow<LanguageNotConfiguredException>();
            ThenAllReadersShouldBeReadTimes(1);
            ThenReadTranslations.Should().BeEquivalentTo(new Dictionary<string, string>
            {
                { "Hello", "Hello1" }
            });
        }

        private void GivenTranslationReaderThrows(string name, Exception exception)
        {
            SetupReader(name, reader =>
            {
                reader
                    .Setup(r => r.Read(It.IsAny<string>()))
                    .Throws(exception);
            });
        }

        private void GivenTranslationReaderReturns(string name, TranslationsConfig translationConfig)
        {
            SetupReader(name, reader =>
            {
                reader
                    .Setup(r => r.Read(It.Is<string>(x => x == translationConfig.Language)))
                    .Returns(translationConfig);
            });
        }

        private void WhenReadTranslationsIsCalled(string language)
        {
            var provider = CreateProvider();
            ThenReadTranslations = provider.ReadTranslations(language).ToList();
        }

        private TranslationsProvider CreateProvider()
        {
            return new TranslationsProvider(
                _readers.Values.Select(x => x.Object).ToList(),
                _options,
                _cache.Object);
        }

        private void ThenAllReadersShouldBeReadTimes(int times)
        {
            foreach (var reader in _readers.Values)
            {
                reader.Verify(
                    r => r.Read(It.IsAny<string>()),
                    Times.Exactly(times));
            }
        }
        
        private void SetupReader(string name, Action<Mock<ITranslationsReader>> setupReader) {
            var readerExists = _readers.TryGetValue(name, out var foundReader);
            var reader = foundReader ?? new Mock<ITranslationsReader>();
            setupReader(reader);
            if (!readerExists)
            {
                _readers.Add(name, reader);
            }
        }
    }
}
