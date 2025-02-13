﻿using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;

using LinqToDB;
using LinqToDB.Mapping;
using LinqToDB.Extensions;

using NUnit.Framework;

namespace Tests.Linq
{
	using LinqToDB.Common;
	using LinqToDB.Data;
	using Model;

	[TestFixture]
	public class TypesTests : TestBase
	{
		[Test]
		public void Bool1([DataSources] string context)
		{
			var value = true;

			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Parent where p.ParentID > 2 && value && true && !false select p,
					from p in db.Parent where p.ParentID > 2 && value && true && !false select p);
		}

		[Test]
		public void Bool2([DataSources] string context)
		{
			var value = true;

			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Parent where p.ParentID > 2 && value || true && !false select p,
					from p in db.Parent where p.ParentID > 2 && value || true && !false select p);
		}

		[Test]
		public void Bool3([DataSources] string context)
		{
			var values = Array<int>.Empty;

			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Parent where values.Contains(p.ParentID) && !false || p.ParentID > 2 select p,
					from p in db.Parent where values.Contains(p.ParentID) && !false || p.ParentID > 2 select p);
		}

		[Test]
		public void BoolField1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Types where t.BoolValue select t.MoneyValue,
					from t in db.Types where t.BoolValue select t.MoneyValue);
		}

		[Test]
		public void BoolField2([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Types where !t.BoolValue select t.MoneyValue,
					from t in db.Types where !t.BoolValue select t.MoneyValue);
		}

		[Test]
		public void BoolField3([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Types where t.BoolValue == true select t.MoneyValue,
					from t in db.Types where t.BoolValue == true select t.MoneyValue);
		}

		[Test]
		public void BoolField4([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Types where t.BoolValue == false select t.MoneyValue,
					from t in db.Types where t.BoolValue == false select t.MoneyValue);
		}

		[Test]
		public void BoolField5([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in from t in    Types select new { t.MoneyValue, b = !t.BoolValue } where p.b == false select p.MoneyValue,
					from p in from t in db.Types select new { t.MoneyValue, b = !t.BoolValue } where p.b == false select p.MoneyValue);
		}

		[Test]
		public void BoolField6([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in from t in    Types select new { t.MoneyValue, b = !t.BoolValue } where p.b select p.MoneyValue,
					from p in from t in db.Types select new { t.MoneyValue, b = !t.BoolValue } where p.b select p.MoneyValue);
		}

		[Test]
		public void BoolResult1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Person select new { p.Patient, IsPatient = p.Patient != null },
					from p in db.Person select new { p.Patient, IsPatient = p.Patient != null });
		}

		[ActiveIssue("https://github.com/Octonica/ClickHouseClient/issues/56 + https://github.com/ClickHouse/ClickHouse/issues/37999", Configurations = new[] { ProviderName.ClickHouseMySql, ProviderName.ClickHouseOctonica })]
		[Test]
		public void BoolResult2([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Person select new { IsPatient = Sql.AsSql(p.Patient != null) },
					from p in db.Person select new { IsPatient = Sql.AsSql(p.Patient != null) });
		}

		[ActiveIssue("https://github.com/Octonica/ClickHouseClient/issues/56 + https://github.com/ClickHouse/ClickHouse/issues/37999", Configurations = new[] { ProviderName.ClickHouseMySql, ProviderName.ClickHouseOctonica })]
		[Test]
		public void BoolResult3([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Person select Sql.AsSql(p.ID == 1),
					from p in db.Person select Sql.AsSql(p.ID == 1));
		}

		[Test]
		public void GuidNew([DataSources] string context)
		{
			using (new DisableBaseline("Server-side guid generation test"))
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Types where p.GuidValue != Guid.NewGuid() select p.GuidValue,
					from p in db.Types where p.GuidValue != Guid.NewGuid() select p.GuidValue);
		}

		[Test]
		public void Guid1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Types where p.GuidValue == new Guid("D2F970C0-35AC-4987-9CD5-5BADB1757436") select p.GuidValue,
					from p in db.Types where p.GuidValue == new Guid("D2F970C0-35AC-4987-9CD5-5BADB1757436") select p.GuidValue);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void Guid2([DataSources] string context)
		{
			var guid3 = new Guid("D2F970C0-35AC-4987-9CD5-5BADB1757436");
			var guid4 = new Guid("40932fdb-1543-4e4a-ac2c-ca371604fb4b");

			var parm = Expression.Parameter(typeof(LinqDataTypes), "p");

			using (var db = GetDataContext(context))
				Assert.AreNotEqual(
					db.Types
						.Where(
							Expression.Lambda<Func<LinqDataTypes,bool>>(
								Expression.Equal(
									Expression.PropertyOrField(parm, "GuidValue"),
									Expression.Constant(guid3),
									false,
									typeof(Guid).GetMethodEx("op_Equality")),
								new[] { parm }))
						.Single().GuidValue,
					db.Types
						.Where(
							Expression.Lambda<Func<LinqDataTypes,bool>>(
								Expression.Equal(
									Expression.PropertyOrField(parm, "GuidValue"),
									Expression.Constant(guid4),
									false,
									typeof(Guid).GetMethodEx("op_Equality")),
								new[] { parm }))
						.Single().GuidValue);
		}

		[Test]
		public void ContainsGuid([DataSources] string context)
		{
			var ids = new [] { new Guid("D2F970C0-35AC-4987-9CD5-5BADB1757436") };

			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Types where ids.Contains(p.GuidValue) select p.GuidValue,
					from p in db.Types where ids.Contains(p.GuidValue) select p.GuidValue);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void NewGuid(
			[DataSources(
				ProviderName.DB2,
				TestProvName.AllInformix,
				TestProvName.AllFirebird,
				TestProvName.AllPostgreSQL,
				TestProvName.AllSQLite,
				TestProvName.AllAccess,
				TestProvName.AllSapHana)]
			string context)
		{
			using (new DisableBaseline("Server-side guid generation test"))
			using (var db = GetDataContext(context))
			using (new RestoreBaseTables(db))
			{
				db.Types.Insert(() => new LinqDataTypes
				{
					ID            = 1001,
					MoneyValue    = 1001,
					DateTimeValue = Sql.CurrentTimestamp,
					BoolValue     = true,
					GuidValue     = Sql.NewGuid(),
					BinaryValue   = new Binary(new byte[] { 1 }),
					SmallIntValue = 1001
				});

				var guid = db.Types.Single(_ => _.ID == 1001).GuidValue;

				Assert.AreEqual(1001, db.Types.Single(_ => _.GuidValue == guid).ID);
			}
		}

		[Test]
		public void BinaryLength([DataSources(TestProvName.AllAccess)] string context)
		{
			using (var db = GetDataContext(context))
			{
				db.Types
					.Where(t => t.ID == 1)
					.Set(t => t.BinaryValue, new Binary(new byte[] { 1, 2, 3, 4, 5 }))
					.Update();

				Assert.That(
					(from t in db.Types where t.ID == 1 select t.BinaryValue!.Length).First(),
					Is.EqualTo(5));

				db.Types
					.Where(t => t.ID == 1)
					.Set(t => t.BinaryValue, (Binary?)null)
					.Update();
			}
		}

		[Test]
		public void InsertBinary1(
			[DataSources(
				ProviderName.DB2,
				TestProvName.AllInformix,
				TestProvName.AllSQLite,
				ProviderName.Access)]
			string context)
		{
			using (var db = GetDataContext(context))
			{
				Binary? data = null;

				db.Types.Delete(_ => _.ID > 1000);
				db.Types.Insert(() => new LinqDataTypes
				{
					ID          = 1001,
					BinaryValue = data,
					BoolValue   = true,
				});
				db.Types.Delete(_ => _.ID > 1000);
			}
		}

		[Test]
		public void UpdateBinary1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			using (new RestoreBaseTables(db))
			{
				db.Types
					.Where(t => t.ID == 1)
					.Set(t => t.BinaryValue, new Binary(new byte[] { 1, 2, 3, 4, 5 }))
					.Update();

				var g = from t in db.Types where t.ID == 1 select t.BinaryValue;

				foreach (var binary in g)
				{
				}
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void UpdateBinary2([DataSources(ProviderName.SqlCe)] string context)
		{
			using (var db = GetDataContext(context))
			using (new RestoreBaseTables(db))
			{
				var ints     = new[] { 1, 2 };
				var binaries = new[] { new byte[] { 1, 2, 3, 4, 5 }, new byte[] { 5, 4, 3, 2, 1 } };

				for (var i = 1; i <= 2; i++)
				{
					db.Types
						.Where(t => t.ID == ints[i - 1])
						.Set(t => t.BinaryValue, binaries[i - 1])
						.Update();
				}

				var g = from t in db.Types where new[] { 1, 2 }.Contains(t.ID) select t;

				foreach (var binary in g)
					Assert.AreEqual(binaries[binary.ID - 1], binary.BinaryValue!.ToArray());
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTime1([DataSources] string context)
		{
			var dt = Types2[3].DateTimeValue;

			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where t.DateTimeValue!.Value.Date > dt!.Value.Date select t),
											from t in db.Types2 where t.DateTimeValue!.Value.Date > dt!.Value.Date select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTime21([DataSources(TestProvName.AllSQLite)] string context)
		{
			using (var db = GetDataContext(context))
			{
				var pdt = db.Types2.First(t => t.ID == 1).DateTimeValue;
				var dt  = DateTime.Parse("2010-12-14T05:00:07.4250141Z");

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue = dt });

				var dt2 = db.Types2.First(t => t.ID == 1).DateTimeValue;

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue = pdt });

				Assert.AreNotEqual(dt.Ticks, dt2!.Value.Ticks);
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTime22(
			[DataSources(
				ProviderName.SqlCe,
				TestProvName.AllAccess,
				TestProvName.AllSqlServer2005,
				ProviderName.DB2,
				TestProvName.AllInformix,
				TestProvName.AllFirebird,
				TestProvName.AllOracle,
				TestProvName.AllPostgreSQL,
				TestProvName.AllMySql,
				TestProvName.AllSybase,
				TestProvName.AllSapHana)]
			string context)
		{
			using (var db = GetDataContext(context))
			{
				var pdt = db.Types2.First(t => t.ID == 1).DateTimeValue2;
				var dt  = DateTime.Parse("2010-12-14T05:00:07.4250141Z");

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue2 = dt });

				var dt2 = db.Types2.First(t => t.ID == 1).DateTimeValue2;

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue2 = pdt });

				Assert.AreEqual(dt, dt2);
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTime23(
			[DataSources(
				ProviderName.SqlCe,
				TestProvName.AllAccess,
				TestProvName.AllSqlServer2005,
				ProviderName.DB2,
				TestProvName.AllInformix,
				TestProvName.AllFirebird,
				TestProvName.AllOracle,
				TestProvName.AllPostgreSQL,
				TestProvName.AllMySql,
				TestProvName.AllSybase,
				TestProvName.AllSapHana)]
			string context)
		{
			using (var db = GetDataContext(context))
			{
				var pdt = db.Types2.First(t => t.ID == 1).DateTimeValue2;
				var dt  = DateTime.Parse("2010-12-14T05:00:07.4250141Z");

				db.Types2
					.Where(t => t.ID == 1)
					.Set  (_ => _.DateTimeValue2, dt)
					.Update();

				var dt2 = db.Types2.First(t => t.ID == 1).DateTimeValue2;

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue2 = pdt });

				Assert.AreEqual(dt, dt2);
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTime24(
			[DataSources(
				ProviderName.SqlCe,
				TestProvName.AllAccess,
				TestProvName.AllSqlServer2005,
				ProviderName.DB2,
				TestProvName.AllInformix,
				TestProvName.AllFirebird,
				TestProvName.AllOracle,
				TestProvName.AllPostgreSQL,
				TestProvName.AllMySql,
				TestProvName.AllSybase,
				TestProvName.AllSapHana)]
			string context)
		{
			using (var db = GetDataContext(context))
			{
				var pdt = db.Types2.First(t => t.ID == 1).DateTimeValue2;
				var dt  = DateTime.Parse("2010-12-14T05:00:07.4250141Z");
				var tt  = db.Types2.First(t => t.ID == 1);

				tt.DateTimeValue2 = dt;

				db.Update(tt);

				var dt2 = db.Types2.First(t => t.ID == 1).DateTimeValue2;

				db.Types2.Update(t => t.ID == 1, t => new LinqDataTypes2 { DateTimeValue2 = pdt });

				Assert.AreEqual(dt, dt2);
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTimeArray1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where new DateTime?[] { new DateTime(2001, 1, 11, 1, 11, 21, 100) }.Contains(t.DateTimeValue) select t),
											from t in db.Types2 where new DateTime?[] { new DateTime(2001, 1, 11, 1, 11, 21, 100) }.Contains(t.DateTimeValue) select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTimeArray2([DataSources(ProviderName.Access)] string context)
		{
			var arr = new DateTime?[] { new DateTime(2001, 1, 11, 1, 11, 21, 100), new DateTime(2012, 11, 7, 19, 19, 29, 90) };

			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where arr.Contains(t.DateTimeValue) select t),
											from t in db.Types2 where arr.Contains(t.DateTimeValue) select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void DateTimeArray3([DataSources(ProviderName.Access)] string context)
		{
			var arr = new List<DateTime?> { new DateTime(2001, 1, 11, 1, 11, 21, 100) };

			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where arr.Contains(t.DateTimeValue) select t),
											from t in db.Types2 where arr.Contains(t.DateTimeValue) select t);
		}

		[Test]
		public void DateTimeParams([DataSources] string context)
		{
			var arr = new List<DateTime?>
			{
				new DateTime(1992, 1, 11, 1, 11, 21, 100),
				new DateTime(1993, 1, 11, 1, 11, 21, 100)
			};

			using (var db = GetDataContext(context))
			{
				foreach (var dateTime in arr)
				{
					var dt = DateTimeParams(db, dateTime);
					Assert.AreEqual(dateTime, dt);
				}
			}
		}

		static DateTime DateTimeParams(ITestDataContext db, DateTime? dateTime)
		{
			var q =
				from t in db.Types2
				where t.DateTimeValue > dateTime
				select new
					{
						t.DateTimeValue,
						dateTime!.Value
					};

			return q.First().Value;
		}

		[Test]
		public void Nullable([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from p in    Parent select new { Value = p.Value1.GetValueOrDefault() },
					from p in db.Parent select new { Value = p.Value1.GetValueOrDefault() });
		}

		[Test]
		public void Unicode([DataSources(
			TestProvName.AllInformix, TestProvName.AllFirebird, TestProvName.AllSybase)]
			string context)
		{
			using (var db = GetDataContext(context))
			using (new RestoreBaseTables(db))
			{
				db.BeginTransaction();

				db.Insert(new Person()
				{
					ID        = 100,
					FirstName = "擊敗奴隸",
					LastName  = "Юникодкин",
					Gender    = Gender.Male
				});

				var person = db.Person.Single(p => p.FirstName == "擊敗奴隸" && p.LastName == "Юникодкин");

				Assert.NotNull (person);
				Assert.AreEqual("擊敗奴隸", person.FirstName);
				Assert.AreEqual("Юникодкин", person.LastName);
			}
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void TestCultureInfo([DataSources(TestProvName.AllInformix)] string context)
		{
			var current = Thread.CurrentThread.CurrentCulture;

			Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Types where t.MoneyValue > 0.5m select t,
					from t in db.Types where t.MoneyValue > 0.5m select t);

			Thread.CurrentThread.CurrentCulture = current;
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void SmallInt([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					from t1 in GetTypes(context)
					join t2 in GetTypes(context) on t1.SmallIntValue equals t2.ID
					select t1
					,
					from t1 in db.Types
					join t2 in db.Types on t1.SmallIntValue equals t2.ID
					select t1);
		}

		[Table("Person", IsColumnAttributeRequired=false)]
		public class PersonCharTest
		{
			public int     PersonID;
			public string  FirstName = null!;
			public string  LastName  = null!;
			public string? MiddleName;
			public char    Gender;
		}

		[Test]
		public void CharTest11([DataSources] string context)
		{
			List<PersonCharTest> list;

			using (var db = new DataConnection())
				list = db.GetTable<PersonCharTest>().ToList();

			using (var db = GetDataContext(context))
				AreEqual(
					from p in list                          where p.Gender == 'M' select p.PersonID,
					from p in db.GetTable<PersonCharTest>() where p.Gender == 'M' select p.PersonID);
		}

		[Test]
		public void CharTest12([DataSources] string context)
		{
			List<PersonCharTest> list;

			using (var db = new DataConnection())
				list = db.GetTable<PersonCharTest>().ToList();

			using (var db = GetDataContext(context))
				AreEqual(
					from p in list                          where p.Gender == 77 select p.PersonID,
					from p in db.GetTable<PersonCharTest>() where p.Gender == 77 select p.PersonID);
		}

		[Test]
		public void CharTest2([DataSources] string context)
		{
			List<PersonCharTest> list;

			using (var db = new DataConnection())
				list = db.GetTable<PersonCharTest>().ToList();

			using (var db = GetDataContext(context))
				AreEqual(
					from p in list                          where 'M' == p.Gender select p.PersonID,
					from p in db.GetTable<PersonCharTest>() where 'M' == p.Gender select p.PersonID);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void BoolTest31([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where (t.BoolValue ?? false) select t),
											from t in db.Types2 where t.BoolValue!.Value      select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void BoolTest32([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where (t.BoolValue ?? false) select t),
											from t in db.Types2 where t.BoolValue == true    select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void BoolTest33([DataSources] string context)
		{
			using (var db = GetDataContext(context))
				AreEqual(
					AdjustExpectedData(db,	from t in    Types2 where (t.BoolValue ?? false) select t),
											from t in db.Types2 where true == t.BoolValue    select t);
		}

		[Test]
		public void LongTest1([DataSources] string context)
		{
			using (var db = GetDataContext(context))
			{
				uint value = 0;

				var q =
					from t in db.Types2
					where t.BigIntValue == value
					select t;

				q.ToList();
			}
		}

		[Test]
		public void CompareNullableInt([DataSources] string context)
		{
			int? param = null;

			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Parent where param == null || t.Value1 == param select t,
					from t in db.Parent where param == null || t.Value1 == param select t);

			param = 1;

			using (var db = GetDataContext(context))
				AreEqual(
					from t in    Parent where param == null || t.Value1 == param select t,
					from t in db.Parent where param == null || t.Value1 == param select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void CompareNullableBoolean1([DataSources] string context)
		{
			bool? param = null;

			using (var db = GetDataContext(context))
				AreEqual(
					from t in GetTypes(context) where param == null || t.BoolValue == param select t,
					from t in db.Types          where param == null || t.BoolValue == param select t);

			param = true;

			using (var db = GetDataContext(context))
				AreEqual(
					from t in GetTypes(context) where param == null || t.BoolValue == param select t,
					from t in db.Types          where param == null || t.BoolValue == param select t);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void CompareNullableBoolean2([DataSources] string context)
		{
			short? param1 = null;
			bool?  param2 = null;

			using (var db = GetDataContext(context))
				AreEqual(
					from t1 in GetTypes(context)
					join t2 in GetTypes(context) on t1.ID equals t2.ID
					where (param1 == null || t1.SmallIntValue == param1) && (param2 == null || t1.BoolValue == param2)
					select t1
					,
					from t1 in db.Types
					join t2 in db.Types on t1.ID equals t2.ID
					where (param1 == null || t1.SmallIntValue == param1) && (param2 == null || t1.BoolValue == param2)
					select t1);

			//param1 = null;
			param2 = false;

			using (var db = GetDataContext(context))
				AreEqual(
					from t1 in GetTypes(context)
					join t2 in GetTypes(context) on t1.ID equals t2.ID
					where (param1 == null || t1.SmallIntValue == param1) && (param2 == null || t1.BoolValue == param2)
					select t1
					,
					from t1 in db.Types
					join t2 in db.Types on t1.ID equals t2.ID
					where (param1 == null || t1.SmallIntValue == param1) && (param2 == null || t1.BoolValue == param2)
					select t1);
		}

		[ActiveIssue("https://github.com/ClickHouse/ClickHouse/issues/37999", Configuration = ProviderName.ClickHouseMySql)]
		[Test]
		public void CompareNullableBoolean3([DataSources] string context)
		{
			short? param1 = null;
			bool?  param2 = false;

			using (var db = GetDataContext(context))
				AreEqual(
					from t in GetTypes(context) where (param1 == null || t.SmallIntValue == param1) && (param2 == null || t.BoolValue == param2) select t,
					from t in db.Types          where (param1 == null || t.SmallIntValue == param1) && (param2 == null || t.BoolValue == param2) select t);
		}

		// AllTypes is mess...
		[Table]
		[Table("ALLTYPES", Configuration = ProviderName.DB2)]
		sealed class AllTypes
		{
			[Column] public int     ID             { get; set; }

			[Column]
			[Column("REALDATATYPE", Configuration = ProviderName.DB2)]
			[Column("realDataType", Configuration = ProviderName.Informix)]
			[Column("realDataType", Configuration = ProviderName.Oracle)]
			[Column("realDataType", Configuration = ProviderName.PostgreSQL)]
			[Column("realDataType", Configuration = ProviderName.SapHana)]
			[Column("realDataType", Configuration = ProviderName.SqlCe)]
			[Column("realDataType", Configuration = ProviderName.SqlServer)]
			[Column("realDataType", Configuration = ProviderName.Sybase)]
			public float? floatDataType { get; set; }


			[Column]
			[Column("DOUBLEDATATYPE", Configuration = ProviderName.DB2)]
			[Column("realDataType"  , Configuration = ProviderName.Access)]
			[Column("realDataType"  , Configuration = ProviderName.SQLite)]
			[Column("floatDataType" , Configuration = ProviderName.Informix)]
			[Column("floatDataType" , Configuration = ProviderName.Oracle)]
			[Column("floatDataType" , Configuration = ProviderName.SapHana)]
			[Column("floatDataType" , Configuration = ProviderName.SqlCe)]
			[Column("floatDataType" , Configuration = ProviderName.SqlServer)]
			[Column("floatDataType" , Configuration = ProviderName.Sybase)]
			public double? doubleDataType { get; set; }
		}

		[Test]
		public void TestSpecialValues(
			[DataSources(
				TestProvName.AllSQLite,
				TestProvName.AllAccess,
				TestProvName.AllInformix,
				TestProvName.AllSybase,
				TestProvName.AllSqlServer,
				TestProvName.AllMySql,
				TestProvName.AllSapHana,
				ProviderName.DB2,
				// SQL CE allows special values using parameters, but no idea how to generate them as literal
				ProviderName.SqlCe
				)] string context,
			[Values] bool inline)
		{
			// TODO: update condition to include only Firebird 2.5 when
			// https://github.com/FirebirdSQL/firebird/issues/6750 releases
			var skipFloatInf = context.IsAnyOf(TestProvName.AllFirebird) && inline;
			var skipId       = context.IsAnyOf(ProviderName.DB2) || context.IsAnyOf(TestProvName.AllSybase) || context.IsAnyOf(ProviderName.SqlCe);

			using (var db = GetDataContext(context))
			using (new RestoreBaseTables(db))
			{
				db.InlineParameters = inline;

				var maxID = db.GetTable<AllTypes>().Select(_ => _.ID).Max();
				var real  = float.NaN;
				var dbl   = double.NaN;
				if (skipId)
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						floatDataType = real,
						doubleDataType = dbl,
					});
				else
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						ID             = 1000,
						floatDataType  = real,
						doubleDataType = dbl,
					});
				real = skipFloatInf ? float.NaN : float.NegativeInfinity;
				dbl  = double.NegativeInfinity;
				if (skipId)
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						floatDataType  = real,
						doubleDataType = dbl,
					});
				else
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						ID             = 1001,
						floatDataType  = real,
						doubleDataType = dbl,
					});
				real = skipFloatInf ? float.NaN : float.PositiveInfinity;
				dbl  = double.PositiveInfinity;
				if (skipId)
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						floatDataType  = real,
						doubleDataType = dbl,
					});
				else
					db.GetTable<AllTypes>().Insert(() => new AllTypes()
					{
						ID             = 1002,
						floatDataType  = real,
						doubleDataType = dbl,
					});

				var res = db.GetTable<AllTypes>()
					.Where(_ => _.ID > maxID)
					.OrderBy(_ => _.ID)
					.Select(_ => new { _.floatDataType, _.doubleDataType})
					.ToArray();

				Assert.AreEqual (3   , res.Length);
				Assert.IsNaN    (res[0].floatDataType);
				Assert.IsNaN    (res[0].doubleDataType);

				Assert.IsNotNull(res[1].floatDataType);
				Assert.IsNotNull(res[1].doubleDataType);
				if (skipFloatInf)
					Assert.IsNaN(res[0].floatDataType);
				else
					Assert.True(float.IsNegativeInfinity(res[1].floatDataType!.Value));
				Assert.True(double.IsNegativeInfinity(res[1].doubleDataType!.Value));

				Assert.IsNotNull(res[2].floatDataType);
				Assert.IsNotNull(res[2].doubleDataType);
				if (skipFloatInf)
					Assert.IsNaN(res[0].floatDataType);
				else
					Assert.True     (float.IsPositiveInfinity(res[2].floatDataType!.Value));
				Assert.True     (double.IsPositiveInfinity(res[2].doubleDataType!.Value));
			}
		}

	}
}
