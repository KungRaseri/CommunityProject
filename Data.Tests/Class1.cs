﻿using System;
using Data.Helpers;
using Data.Models;
using NUnit.Framework;

namespace Data.Tests
{
    [TestFixture]
    public class CouchDbStoreTests
    {
        private CouchDbStore<User> _usersCollection { get; set; }
        private CouchDbStore<Settings> _settingsCollection { get; set; }
        private Settings _settings;

        [SetUp]
        public void SetUpTests()
        {
            _settingsCollection = new CouchDbStore<Settings>("http://root:123456789@localhost:5984/");
            _settings = _settingsCollection.GetAsync("9c3131ee7b9fb97491e8551211495381").GetAwaiter().GetResult();

            _usersCollection = new CouchDbStore<User>(_settings.CouchDbUri);
        }

        [Test]
        public void FindUserByEmail_ReturnsUser()
        {
            var email = "damastaSlayer@monkasthatwasn'tgood.com";

            var user = _usersCollection.FindUserByEmail(email).GetAwaiter().GetResult();

            Assert.That(user, Is.TypeOf<User>());
            Assert.That(user.Email, Is.EqualTo(email));
        }

        [Test]
        public void FindUserByEmail_UserDoesNotExist_ReturnsNull()
        {
            var user = _usersCollection.FindUserByEmail(string.Empty).GetAwaiter().GetResult();

            Assert.That(user, Is.Null);
        }
    }
}
