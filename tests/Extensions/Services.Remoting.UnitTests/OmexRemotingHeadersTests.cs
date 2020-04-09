﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Omex.Extensions.Services.Remoting;
using Microsoft.ServiceFabric.Services.Remoting.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Services.Remoting
{
	[TestClass]
	public class OmexRemotingHeadersTests
	{
		[TestMethod]
		public void OmexRemotingHeaders_AttachActivityToOuthgoingRequest_HandlessNullActivityProperly()
		{
			Mock<IServiceRemotingRequestMessage> requestMock = new Mock<IServiceRemotingRequestMessage>();
			OmexRemotingHeaders.AttachActivityToOuthgoingRequest(null, requestMock.Object);
		}

		[TestMethod]
		public void OmexRemotingHeaders_ExtractActivityFromIncominRequest_HandlessNullActivityProperly()
		{
			Mock<IServiceRemotingRequestMessage> requestMock = new Mock<IServiceRemotingRequestMessage>();
			OmexRemotingHeaders.ExtractActivityFromIncominRequest(null, requestMock.Object);
		}

		[TestMethod]
		public void OmexRemotingHeaders_WithoutBaggage_ProperlyTransfered()
		{
			Activity outgoingActivity = new Activity(nameof(OmexRemotingHeaders_WithoutBaggage_ProperlyTransfered));
			TestActivityTransfer(outgoingActivity);
		}

		[TestMethod]
		public void OmexRemotingHeaders_WithBaggage_ProperlyTransfered()
		{
			Activity outgoingActivity = new Activity(nameof(OmexRemotingHeaders_WithBaggage_ProperlyTransfered))
				.AddBaggage("NullValue", null)
				.AddBaggage("EmptyValue", string.Empty)
				.AddBaggage("TestValue", "Value @+&")
				.AddBaggage("UnicodeValue", "☕☘☔ (┛ಠ_ಠ)┛彡┻━┻");

			TestActivityTransfer(outgoingActivity);
		}

		private void TestActivityTransfer(Activity outgoingActivity)
		{
			HeaderMock header = new HeaderMock();
			Mock<IServiceRemotingRequestMessage> requestMock = new Mock<IServiceRemotingRequestMessage>();
			requestMock.Setup(m => m.GetHeader()).Returns(header);

			outgoingActivity.Start();
			OmexRemotingHeaders.AttachActivityToOuthgoingRequest(outgoingActivity, requestMock.Object);
			outgoingActivity.Stop();

			Activity incomingActivity = new Activity(outgoingActivity.OperationName + "_Out").Start();
			OmexRemotingHeaders.ExtractActivityFromIncominRequest(incomingActivity, requestMock.Object);
			incomingActivity.Stop();

			Assert.AreEqual(outgoingActivity.Id, incomingActivity.ParentId);
			CollectionAssert.AreEqual(outgoingActivity.Baggage.ToArray(), incomingActivity.Baggage.ToArray());
		}

		public class HeaderMock : IServiceRemotingRequestMessageHeader
		{
			public int MethodId { get; set; }

			public int InterfaceId { get; set; }

			public string? InvocationId { get; set; }

			public string? MethodName { get; set; }

			private readonly Dictionary<string, byte[]> m_headers = new Dictionary<string, byte[]>();

			public void AddHeader(string headerName, byte[] headerValue) => m_headers.Add(headerName, headerValue);

			public bool TryGetHeaderValue(string headerName, [MaybeNullWhen(false)] out byte[] headerValue) => m_headers.TryGetValue(headerName, out headerValue);
		}
	}
}
