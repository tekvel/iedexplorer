﻿/*
 *  Copyright (C) 2013 Pavel Charvat
 * 
 *  This file is part of IEDExplorer.
 *
 *  IEDExplorer is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *
 *  IEDExplorer is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *
 *  You should have received a copy of the GNU General Public License
 *  along with IEDExplorer.  If not, see <http://www.gnu.org/licenses/>.
 */

using System;
using System.Collections.Generic;
using org.bn.attributes;
using org.bn.attributes.constraints;
using org.bn.coders;
using org.bn.types;
using org.bn;
using MMS_ASN1_Model;
using System.IO;
using System.Text;

namespace IEDExplorer
{
    public class Scsm_MMS
    {
        // Protocol IEC6850 - definitions
        // OptFlds - report Optional Fields
        // 1st Byte
        public const byte OptFldsReserved = 0x80; // bit "0" in MMS interpretation
        public const byte OptFldsSeqNum = 0x40;
        public const byte OptFldsTimeOfEntry = 0x20;
        public const byte OptFldsReasonCode = 0x10;
        public const byte OptFldsDataSet = 0x08;
        public const byte OptFldsDataReference = 0x04;
        public const byte OptFldsOvfl = 0x02;
        public const byte OptFldsEntryID = 0x01;
        // 2nd Byte
        public const byte OptFldsConfRev = 0x80;
        public const byte OptFldsMoreSegments = 0x40; // bit "10" in MMS interpretation

        // TrgOps - report Trigger Options
        public const byte TrgOpsReserved = 0x80;	// bit "0" in MMS interpretation
        public const byte TrgOpsDataChange = 0x40;
        public const byte TrgOpsQualChange = 0x20;
        public const byte TrgOpsDataActual = 0x10;
        public const byte TrgOpsIntegrity = 0x08;
        public const byte TrgOpsGI = 0x04; // bit "6" in MMS interpretation

        // DatQual - Data Quality Codes
        public const byte DatQualValidity0 = 0x80; // bit "0" in MMS interpretation
        public const byte DatQualValidity1 = 0x40;
        public const byte DatQualOverflow = 0x20;
        public const byte DatQualOutOfRange = 0x10;
        public const byte DatQualBadReference = 0x08;
        public const byte DatQualOscillatory = 0x04;
        public const byte DatQualFailure = 0x02;
        public const byte DatQualOldData = 0x01;
        // 2nd Byte
        public const byte DatQualInconsistent = 0x80;
        public const byte DatQualInaccurate = 0x40;
        public const byte DatQualSource = 0x20;
        public const byte DatQualTest = 0x10;
        public const byte DatQualOperatorBlocked = 0x08; // bit "12" in MMS interpretation

        // TimQual - Time Quality Codes
        public const byte TimQualExtraSeconds = 0x80; // bit "0" in MMS interpretation
        public const byte TimQualTimeBaseErr = 0x40;
        public const byte TimQualNotSynchronized = 0x20; // bit "2". Bits 3-7=time precision encoding

        // Report Reading Phases
        const int phsRptID = 0;
        const int phsOptFlds = 1;
        const int phsSeqNum = 2;
        const int phsTimeOfEntry = 3;
        const int phsDatSet = 4;
        const int phsBufOvfl = 5;
        const int phsEntryID = 6;
        const int phsConfRev = 7;
        const int phsSubSeqNr = 8;
        const int phsMoreSegmentsFollow = 9;
        const int phsInclusionBitstring = 10;
        const int phsDataReferences = 11;
        const int phsValues = 12;
        const int phsReasonCodes = 13;

        IDecoder decoder = CoderFactory.getInstance().newDecoder("BER");
        IEncoder encoder = CoderFactory.getInstance().newEncoder("BER");

        int InvokeID = 0;
        int MaxCalls = 10;

        bool[] ServiceSupportOptions = new bool[96];
        enum ServiceSupportOptionsEnum
        {
            status = 0,
            getNameList = 1,
            identify = 2,
            rename = 3,
            read = 4,
            write = 5,
            getVariableAccessAttributes = 6,
            defineNamedVariable = 7,
            defineScatteredAccess = 8,
            getScatteredAccessAttributes = 9,
            deleteVariableAccess = 10,
            defineNamedVariableList = 11,
            getNamedVariableListAttributes = 12,
            deleteNamedVariableList = 13,
            defineNamedType = 14,
            getNamedTypeAttributes = 15,
            deleteNamedType = 16,
            input = 17,
            output = 18,
            takeControl = 19,
            relinquishControl = 20,
            defineSemaphore = 21,
            deleteSemaphore = 22,
            reportSemaphoreStatus = 23,
            reportPoolSemaphoreStatus = 24,
            reportSemaphoreEntryStatus = 25,
            initiateDownloadSequence = 26,
            downloadSegment = 27,
            terminateDownloadSequence = 28,
            initiateUploadSequence = 29,
            uploadSegment = 30,
            terminateUploadSequence = 31,
            requestDomainDownload = 32,
            requestDomainUpload = 33,
            loadDomainContent = 34,
            storeDomainContent = 35,
            deleteDomain = 36,
            getDomainAttributes = 37,
            createProgramInvocation = 38,
            deleteProgramInvocation = 39,
            start = 40,
            stop = 41,
            resume = 42,
            reset = 43,
            kill = 44,
            getProgramInvocationAttributes = 45,
            obtainFile = 46,
            defineEventCondition = 47,
            deleteEventCondition = 48,
            getEventConditionAttributes = 49,
            reportEventConditionStatus = 50,
            alterEventConditionMonitoring = 51,
            triggerEvent = 52,
            defineEventAction = 53,
            deleteEventAction = 54,
            getEventActionAttributes = 55,
            reportEventActionStatus = 56,
            defineEventEnrollment = 57,
            deleteEventEnrollment = 58,
            alterEventEnrollment = 59,
            reportEventEnrollmentStatus = 60,
            getEventEnrollmentAttributes = 61,
            acknowledgeEventNotification = 62,
            getAlarmSummary = 63,
            getAlarmEnrollmentSummary = 64,
            readJournal = 65,
            writeJournal = 66,
            initializeJournal = 67,
            reportJournalStatus = 68,
            createJournal = 69,
            deleteJournal = 70,
            getCapabilityList = 71,
            fileOpen = 72,
            fileRead = 73,
            fileClose = 74,
            fileRename = 75,
            fileDelete = 76,
            fileDirectory = 77,
            unsolicitedStatus = 78,
            informationReport = 79,
            eventNotification = 80,
            attachToEventCondition = 81,
            attachToSemaphore = 82,
            conclude = 83,
            cancel = 84
        }

        enum DataAccessError
        {
            objectInvalidated = 0,
            hardwareFault = 1,
            temporarilyUnavailable = 2,
            objectAccessDenied = 3,
            objectUndefined = 4,
            invalidAddress = 5,
            typeUnsupported = 6,
            typeInconsistent = 7,
            objectAttributeInconsistent = 8,
            objectAccessUnsupported = 9,
            objectNonexistent = 10,
            objectValueInvalid = 11
        }

        enum RejectPDU_rejectReason_confirmedRequestPDU
        {
            other = 0,
            unrecognizedService = 1,
            unrecognizedModifier = 2,
            invalidInvokeID = 3,
            invalidArgument = 4,
            invalidModifier = 5,
            maxServOutstandingEexceeded = 6,
            maxRecursionExceeded = 8,
            valueOutOfRange = 9
        }
        enum RejectPDU_rejectReason_confirmedResponsePDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidInvokeID = 2,
            invalidResult = 3,
            maxRecursionExceeded = 5,
            valueOutOfRange = 6
        }
        enum RejectPDU_rejectReason_confirmedErrorPDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidInvokeID = 2,
            invalidServiceError = 3,
            valueOutOfRange = 4
        }
        enum RejectPDU_rejectReason_unconfirmedPDU
        {
            other = 0,
            unrecognizedService = 1,
            invalidArgument = 2,
            maxRecursionExceeded = 3,
            valueOutOfRange = 4
        }
        enum RejectPDU_rejectReason_pduError
        {
            unknownPduType = 0,
            invalidPdu = 1,
            illegalAcseMapping = 2
        }
        enum RejectPDU_rejectReason_concludeRequestPDU
        {
            other = 0,
            invalidArgument = 1
        }
        enum RejectPDU_rejectReason_concludeResponsePDU
        {
            other = 0,
            invalidResult = 1
        }
        enum RejectPDU_rejectReason_concludeErrorPDU
        {
            other = 0,
            invalidServiceError = 1,
            valueOutOfRange = 2
        }

        enum ServiceError_errorClass_vmdstate
        {
            other = 0,
            vmdstateconflict = 1,
            vmdoperationalproblem = 2,
            domaintransferproblem = 3,
            statemachineidinvalid = 4
        }
        enum ServiceError_errorClass_applicationreference
        {
            other = 0,
            aplicationunreachable = 1,
            connectionlost = 2,
            applicationreferenceinvalid = 3,
            contextunsupported = 4
        }
        enum ServiceError_errorClass_definition
        {
            other = 0,
            objectundefined = 1,
            invalidaddress = 2,
            typeunsupported = 3,
            typeinconsistent = 4,
            objectexists = 5,
            objectattributeinconsistent = 6
        }
        enum ServiceError_errorClass_resource
        {
            other = 0,
            memoryunavailable = 1,
            processorresourceunavailable = 2,
            massstorageunavailable = 3,
            capabilityunavailable = 4,
            capabilityunknown = 5
        }
        enum ServiceError_errorClass_service
        {
            other = 0,
            primitivesoutofsequence = 1,
            objectstateconflict = 2,
            pdusize = 3,
            continuationinvalid = 4,
            objectconstraintconflict = 5
        }
        enum ServiceError_errorClass_servicepreempt
        {
            other = 0,
            timeout = 1,
            deadlock = 2,
            cancel = 3
        }
        enum ServiceError_errorClass_timeresolution
        {
            other = 0,
            unsupportabletimeresolution = 1
        }
        enum ServiceError_errorClass_access
        {
            other = 0,
            objectaccessunsupported = 1,
            objectnonexistent = 2,
            objectaccessdenied = 3,
            objectinvalidated = 4
        }
        enum ServiceError_errorClass_initiate
        {
            other = 0,
            versionincompatible = 1,
            maxsegmentinsufficient = 2,
            maxservicesoutstandingcallinginsufficient = 3,
            maxservicesoutstandingcalledinsufficient = 4,
            serviceCBBinsufficient = 5,
            parameterCBBinsufficient = 6,
            nestinglevelinsufficient = 7
        }
        enum ServiceError_errorClass_conclude
        {
            other = 0,
            furthercommunicationrequired = 1
        }
        enum ServiceError_errorClass_cancel
        {
            other = 0,
            invokeidunknown = 1,
            cancelnotpossible = 2
        }
        enum ServiceError_errorClass_file
        {
            other = 0,
            filenameambiguous = 1,
            filebusy = 2,
            filenamesyntaxError = 3,
            contenttypeinvalid = 4,
            positioninvalid = 5,
            fileaccesdenied = 6,
            filenonexistent = 7,
            duplicatefilename = 8,
            insufficientspaceinfilestore = 9
        }

        enum ControlAddCause
        {
            ADD_CAUSE_UNKNOWN = 0, ADD_CAUSE_NOT_SUPPORTED = 1, ADD_CAUSE_BLOCKED_BY_SWITCHING_HIERARCHY = 2, ADD_CAUSE_SELECT_FAILED = 3,
            ADD_CAUSE_INVALID_POSITION = 4, ADD_CAUSE_POSITION_REACHED = 5, ADD_CAUSE_PARAMETER_CHANGE_IN_EXECUTION = 6, ADD_CAUSE_STEP_LIMIT = 7,
            ADD_CAUSE_BLOCKED_BY_MODE = 8, ADD_CAUSE_BLOCKED_BY_PROCESS = 9, ADD_CAUSE_BLOCKED_BY_INTERLOCKING = 10, ADD_CAUSE_BLOCKED_BY_SYNCHROCHECK = 11,
            ADD_CAUSE_COMMAND_ALREADY_IN_EXECUTION = 12, ADD_CAUSE_BLOCKED_BY_HEALTH = 13, ADD_CAUSE_1_OF_N_CONTROL = 14, ADD_CAUSE_ABORTION_BY_CANCEL = 15,
            ADD_CAUSE_TIME_LIMIT_OVER = 16, ADD_CAUSE_ABORTION_BY_TRIP = 17, ADD_CAUSE_OBJECT_NOT_SELECTED = 18, ADD_CAUSE_OBJECT_ALREADY_SELECTED = 19,
            ADD_CAUSE_NO_ACCESS_AUTHORITY = 20, ADD_CAUSE_ENDED_WITH_OVERSHOOT = 21, ADD_CAUSE_ABORTION_DUE_TO_DEVIATION = 22, ADD_CAUSE_ABORTION_BY_COMMUNICATION_LOSS = 23,
            ADD_CAUSE_ABORTION_BY_COMMAND = 24, ADD_CAUSE_NONE = 25, ADD_CAUSE_INCONSISTENT_PARAMETERS = 26, ADD_CAUSE_LOCKED_BY_OTHER_CLIENT = 27
        }

        enum ControlError
        {
            NoError = 0,
            Unknown = 1,
            TimeoutTestNotOk = 2,
            OperatortestNotOk = 3
        }

        static Env _env = Env.getEnv();

        public int ReceiveData(Iec61850State iecs)
        {
            if (iecs == null)
                return -1;

            iecs.logger.LogDebugBuffer("mms.ReceiveData", iecs.msMMS.GetBuffer(), iecs.msMMS.Position, iecs.msMMS.Length - iecs.msMMS.Position);

            MMSpdu mymmspdu = null;
            try
            {
                MMSCapture cap = null;
                byte[] pkt = iecs.msMMS.ToArray();
                if (iecs.CaptureDb.CaptureActive) cap = new MMSCapture(pkt, iecs.msMMS.Position, pkt.Length, MMSCapture.CaptureDirection.In);
                ////////////////// Decoding
                mymmspdu = decoder.decode<MMSpdu>(iecs.msMMS);
                ////////////////// Decoding
                if (iecs.CaptureDb.CaptureActive && mymmspdu != null)
                {
                    cap.MMSPdu = mymmspdu;
                    iecs.CaptureDb.AddPacket(cap);
                }
            }
            catch (Exception e)
            {
                iecs.logger.LogError("mms.ReceiveData: Malformed MMS Packet received!!!: " + e.Message);
            }

            if (mymmspdu == null)
            {
                iecs.logger.LogError("mms.ReceiveData: Parsing Error!");

                // Workaround - we can continue when reading-in the model also if one read fails
                if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT)
                {
                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                    NodeBase logNode = iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().GetActualChildNode();
                    if (logNode != null)
                    {
                        iecs.logger.LogWarning("mms.ReceiveData: Error reading " + logNode.IecAddress + " in IEC61850_READ_MODEL_DATA_WAIT, data values not actual in the subtree!");
                    }
                    // Should be possible in this phase: only 1 request can be pending in the discovery phase
                    iecs.OutstandingCalls.Clear();
                    // Set up the state variable
                    if (iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().NextActualChildNode() == null)
                    {
                        if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                        {
                            if (iecs.DataModel.ied.NextActualChildNode() == null)
                            {
                                // End of loop
                                iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                                iecs.logger.LogInfo("Reading named variable lists: [IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                                iecs.DataModel.ied.ResetAllChildNodes();
                            }
                        }
                    }
                }
                return -1;
            }
            else if (mymmspdu.Initiate_ResponsePDU != null)
            {
                removeCall(iecs, -1);
                ReceiveInitiate(iecs, mymmspdu.Initiate_ResponsePDU);
            }
            else if (mymmspdu.Confirmed_ResponsePDU != null && mymmspdu.Confirmed_ResponsePDU.Service != null)
            {
                iecs.logger.LogDebug("mymmspdu.Confirmed_ResponsePDU.Service exists!");
                NodeBase[] operData = removeCall(iecs, mymmspdu.Confirmed_ResponsePDU.InvokeID.Value);

                if (mymmspdu.Confirmed_ResponsePDU.Service.Identify != null)
                {
                    ReceiveIdentify(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Identify);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.GetNameList != null)
                {
                    ReceiveGetNameList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetNameList);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.GetVariableAccessAttributes != null)
                {
                    ReceiveGetVariableAccessAttributes(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetVariableAccessAttributes);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.GetNamedVariableListAttributes != null)
                {
                    ReceiveGetNamedVariableListAttributes(iecs, mymmspdu.Confirmed_ResponsePDU.Service.GetNamedVariableListAttributes);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.Read != null)
                {
                    ReceiveRead(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Read, operData);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.Write != null)
                {
                    ReceiveWrite(iecs, mymmspdu.Confirmed_ResponsePDU.Service.Write, operData);
                    //iecs.logger.LogError("Not implemented PDU Write response received!!");
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.DefineNamedVariableList != null)
                {
                    ReceiveDefineNamedVariableList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.DefineNamedVariableList, operData);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.FileDirectory != null)
                {
                    ReceiveFileDirectory(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileDirectory);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.FileOpen != null)
                {
                    ReceiveFileOpen(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileOpen);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.FileRead != null)
                {
                    ReceiveFileRead(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileRead);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.FileClose != null)
                {
                    ReceiveFileClose(iecs, mymmspdu.Confirmed_ResponsePDU.Service.FileClose);
                }
                else if (mymmspdu.Confirmed_ResponsePDU.Service.DeleteNamedVariableList != null)
                {
                    ReceiveDeleteNamedVariableList(iecs, mymmspdu.Confirmed_ResponsePDU.Service.DeleteNamedVariableList, operData);
                }
            }
            else if (mymmspdu.Unconfirmed_PDU != null && mymmspdu.Unconfirmed_PDU.Service != null && mymmspdu.Unconfirmed_PDU.Service.InformationReport != null)
            {
                ReceiveInformationReport(iecs, mymmspdu.Unconfirmed_PDU.Service.InformationReport);
            }
            else if (mymmspdu.RejectPDU != null)
            {
                NodeBase[] operData = removeCall(iecs, mymmspdu.RejectPDU.OriginalInvokeID.Value);
                ReceiveRejectPDU(iecs, mymmspdu);
            }
            else if (mymmspdu.Confirmed_ErrorPDU != null)
            {
                NodeBase[] operData = removeCall(iecs, mymmspdu.Confirmed_ErrorPDU.InvokeID.Value);
                iecs.logger.LogError("Confirmed_ErrorPDU received - requested operation not possible!!");
            }
            else
            {
                iecs.logger.LogError("Not implemented PDU received!!");
            }

            return 0;
        }

        private void ReceiveRejectPDU(Iec61850State iecs, MMSpdu mymmspdu)
        {
            string operation = "unknown", reason = "unknown";

            if (mymmspdu.RejectPDU.RejectReason.isCancel_errorPDUSelected())
            {
                operation = "Cancel_errorPDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isCancel_requestPDUSelected())
            {
                operation = "Cancel_requestPDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isCancel_responsePDUSelected())
            {
                operation = "Cancel_responsePDU";
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_errorPDUSelected())
            {
                operation = "Conclude_errorPDU";
                reason = ((RejectPDU_rejectReason_concludeErrorPDU)mymmspdu.RejectPDU.RejectReason.Conclude_errorPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_requestPDUSelected())
            {
                operation = "Conclude_requestPDU";
                reason = ((RejectPDU_rejectReason_concludeRequestPDU)mymmspdu.RejectPDU.RejectReason.Conclude_requestPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConclude_responsePDUSelected())
            {
                operation = "Conclude_responsePDU";
                reason = ((RejectPDU_rejectReason_concludeResponsePDU)mymmspdu.RejectPDU.RejectReason.Conclude_responsePDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_errorPDUSelected())
            {
                operation = "Confirmed_errorPDU";
                reason = ((RejectPDU_rejectReason_confirmedErrorPDU)mymmspdu.RejectPDU.RejectReason.Confirmed_errorPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_requestPDUSelected())
            {
                operation = "Confirmed_requestPDU";
                reason = ((RejectPDU_rejectReason_confirmedRequestPDU)mymmspdu.RejectPDU.RejectReason.Confirmed_requestPDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isConfirmed_responsePDUSelected())
            {
                operation = "Confirmed_responsePDU";
                reason = ((RejectPDU_rejectReason_confirmedResponsePDU)mymmspdu.RejectPDU.RejectReason.Confirmed_responsePDU).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isPdu_errorSelected())
            {
                operation = "Pdu_error";
                reason = ((RejectPDU_rejectReason_pduError)mymmspdu.RejectPDU.RejectReason.Pdu_error).ToString();
            }
            else if (mymmspdu.RejectPDU.RejectReason.isUnconfirmedPDUSelected())
            {
                operation = "UnconfirmedPDU";
                reason = ((RejectPDU_rejectReason_unconfirmedPDU)mymmspdu.RejectPDU.RejectReason.UnconfirmedPDU).ToString();
            }
            iecs.logger.LogError("RejectPDU received - requested operation " + operation + " rejected!! Reason code: " + reason);
        }

        private void ReceiveFileDirectory(Iec61850State iecs, FileDirectory_Response dir)
        {
            iecs.logger.LogInfo("FileDirectory PDU received!!");
            if (dir.ListOfDirectoryEntry != null)
            {
                foreach (DirectoryEntry de in dir.ListOfDirectoryEntry)
                {
                    if (de.FileName != null)
                    {
                        IEnumerator<string> en = de.FileName.Value.GetEnumerator();
                        en.MoveNext();
                        string name = en.Current;
                        bool absolutePath = false;
                        bool isdir = false;
                        NodeFile nf = null;
                        NodeFile nfbase = null;

                        if (name.StartsWith("/") || name.StartsWith("\\"))
                        {
                            absolutePath = true;
                        }
                        if (name.EndsWith("/") || name.EndsWith("\\"))
                        {
                            isdir = true;
                        }

                        string[] names = name.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < names.Length; i++)
                        {
                            bool isdirResult = isdir || (i < (names.Length - 1));
                            if (i == 0) nfbase = nf = new NodeFile(names[i], isdirResult);
                            else nf = (NodeFile)nf.AddChildNode(new NodeFile(names[i], isdirResult));
                        }
                        if (!nf.isDir)
                        {
                            nf.ReportedSize = de.FileAttributes.SizeOfFile.Value;
                            nf.ReportedTime = de.FileAttributes.LastModified;
                        }
                        if (absolutePath)
                        {
                            (iecs.lastFileOperationData[0]).GetIedNode().AddChildNode(nfbase, true);
                        }
                        else
                        {
                            (iecs.lastFileOperationData[0]).AddChildNode(nfbase, true);
                        }
                    }
                    else
                        iecs.logger.LogInfo("Empty FileName in FileDirectory PDU!!");
                }
                if (iecs.lastFileOperationData[0] is NodeFile)
                    (iecs.lastFileOperationData[0] as NodeFile).FileReady = true;
                iecs.fstate = FileTransferState.FILE_DIRECTORY;
            }
            else
                iecs.logger.LogInfo("No file in FileDirectory PDU!!");
        }

        private void ReceiveFileOpen(Iec61850State iecs, FileOpen_Response fileopn)
        {
            iecs.logger.LogInfo("FileOpen PDU received!!");
            if (iecs.lastFileOperationData[0] is NodeFile)
            {
                (iecs.lastFileOperationData[0] as NodeFile).frsmId = fileopn.FrsmID.Value;
                iecs.fstate = FileTransferState.FILE_OPENED;
                iecs.logger.LogInfo("FileOpened: " + (iecs.lastFileOperationData[0] as NodeFile).FullName +
                    " Size: " + fileopn.FileAttributes.SizeOfFile.Value.ToString());
            }
        }

        private void ReceiveFileRead(Iec61850State iecs, FileRead_Response filerd)
        {
            iecs.logger.LogInfo("FileRead PDU received!!");
            if (iecs.lastFileOperationData[0] is NodeFile)
            {
                (iecs.lastFileOperationData[0] as NodeFile).AppendData(filerd.FileData);
                if (filerd.MoreFollows)
                    iecs.fstate = FileTransferState.FILE_READ;
                else
                {
                    iecs.fstate = FileTransferState.FILE_COMPLETE;
                    (iecs.lastFileOperationData[0] as NodeFile).FileReady = true;
                }
            }
        }

        private void ReceiveFileClose(Iec61850State iecs, FileClose_Response filecls)
        {
            iecs.logger.LogInfo("FileClose PDU received!!");
            iecs.fstate = FileTransferState.FILE_NO_ACTION;
        }

        private void ReceiveDefineNamedVariableList(Iec61850State iecs, DefineNamedVariableList_Response dnvl, NodeBase[] lastOperationData)
        {
            NodeVL nvl = (lastOperationData[0] as NodeVL);
            nvl.Defined = true;
            if (nvl.OnDefinedSuccess != null)
                nvl.OnDefinedSuccess(nvl, null);
        }

        private void ReceiveDeleteNamedVariableList(Iec61850State iecs, DeleteNamedVariableList_Response dnvl, NodeBase[] lastOperationData)
        {
            NodeVL nvl = (lastOperationData[0] as NodeVL);
            if (dnvl.NumberMatched.Value == 0)
            {
                Logger.getLogger().LogWarning("NVL name did not match any list on server: " + nvl.Name);
            }
            if (dnvl.NumberDeleted.Value > 0)
            {
                nvl.Defined = false;
                if (nvl.OnDeleteSuccess != null)
                    nvl.OnDeleteSuccess(nvl, null);
            }
            else
                Logger.getLogger().LogWarning("NVL Not deleted on server: " + nvl.Name);
        }

        private void ReceiveWrite(Iec61850State iecs, Write_Response write, NodeBase[] lastOperationData)
        {
            int i = 0;
            try
            {
                foreach (Write_Response.Write_ResponseChoiceType wrc in write.Value)
                {
                    if (wrc.isFailureSelected())
                        Logger.getLogger().LogWarning("Write failed for " + lastOperationData[i++].IecAddress + ", failure: " + wrc.Failure.Value.ToString()
                            + ", (" + Enum.GetName(typeof(DataAccessError), ((DataAccessError)wrc.Failure.Value)) + ")");
                    if (wrc.isSuccessSelected())
                        Logger.getLogger().LogInfo("Write succeeded for " + lastOperationData[i++].IecAddress);
                }
            }
            catch { }
        }

        private void ReceiveInformationReport(Iec61850State iecs, InformationReport Report)
        {
            string rptName, datName = null, varName;
            byte[] rptOpts = new byte[] { 0, 0 };
            byte[] incBstr = new byte[] { 0 };
            List<AccessResult> list;
            int phase = phsRptID;
            int datanum = 0;
            int datacnt = 0;
            int reasoncnt = 0;
            int[] listmap = null;

            iecs.logger.LogDebug("Report != null");
            if (Report.VariableAccessSpecification != null && Report.VariableAccessSpecification.VariableListName != null &&
                Report.VariableAccessSpecification.VariableListName.Vmd_specific != null)
            {
                iecs.logger.LogDebug("Report.VariableAccessSpecification.VariableListName.Vmd_specific = " + Report.VariableAccessSpecification.VariableListName.Vmd_specific.Value);

                if (Report.VariableAccessSpecification.VariableListName.Vmd_specific.Value == "RPT")
                {
                    if (Report.ListOfAccessResult != null)
                    {
                        iecs.logger.LogDebug("Report.ListOfAccessResult != null");
                        list = (List<AccessResult>)Report.ListOfAccessResult;
                        for (int i = 0; i < list.Count; i++)
                        {
                            if (list[i].Success != null)
                            {
                                if (phase == phsRptID)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isVisible_stringSelected())
                                    {
                                        rptName = list[i].Success.Visible_string;
                                        iecs.logger.LogDebug("Report Name = " + rptName);
                                        continue;
                                    }
                                }
                                if (phase == phsOptFlds)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isBit_stringSelected())
                                    {
                                        rptOpts = list[i].Success.Bit_string.Value;
                                        iecs.logger.LogDebug("Report Optional Fields = " + rptOpts[0].ToString());
                                        continue;
                                    }
                                }
                                if (phase == phsSeqNum)
                                {
                                    phase++;
                                    // Is this phase active, e.g. is this bit set in OptFlds??
                                    if ((rptOpts[0] & OptFldsSeqNum) != 0)
                                    {
                                        // No evaluation of Sequence Number
                                        continue;
                                    }
                                }
                                if (phase == phsTimeOfEntry)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsTimeOfEntry) != 0)
                                    {
                                        // No evaluation of Time Of Entry
                                        continue;
                                    }
                                }
                                if (phase == phsDatSet)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsDataSet) != 0)
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            datName = list[i].Success.Visible_string;
                                            iecs.logger.LogDebug("Report Data Set Name = " + datName);
                                            continue;
                                        }
                                }
                                if (phase == phsBufOvfl)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsOvfl) != 0)
                                    {
                                        // No evaluation of rptOptsOvfl
                                        continue;
                                    }
                                }
                                if (phase == phsEntryID)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[0] & OptFldsEntryID) != 0)
                                    {
                                        // No evaluation of OptFldsEntryID
                                        continue;
                                    }
                                }
                                if (phase == phsConfRev)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsConfRev) != 0)
                                    {
                                        // No evaluation of OptFldsConfRev
                                        continue;
                                    }
                                }
                                if (phase == phsSubSeqNr)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsMoreSegments) != 0)
                                    {
                                        // No evaluation of OptFldsMoreSegments
                                        continue;
                                    }
                                }
                                if (phase == phsMoreSegmentsFollow)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    phase++;
                                    if ((rptOpts[1] & OptFldsMoreSegments) != 0)
                                    {
                                        // No evaluation of OptFldsMoreSegments
                                        continue;
                                    }
                                }
                                if (phase == phsInclusionBitstring)
                                { // Is this phase active??
                                    phase++;
                                    if (list[i].Success.isBit_stringSelected())
                                    {
                                        incBstr = list[i].Success.Bit_string.Value;
                                        int effbytes = incBstr.Length - (int)(list[i].Success.Bit_string.TrailBitsCnt / 8);
                                        int effpadding = list[i].Success.Bit_string.TrailBitsCnt % 8;
                                        listmap = new int[incBstr.Length * 8];   // for each data in report includes its index into variable list
                                        int listidx = 0;
                                        for (int j = 0; j < effbytes; j++)
                                        {
                                            int l = 0x0;
                                            if ((effpadding > 0) && (j + 1 == effbytes))
                                                l = 1 << (effpadding - 1);
                                            for (int k = 0x80; k > l; k = k >> 1)
                                            {
                                                if ((incBstr[j] & k) > 0)
                                                {
                                                    listmap[datanum] = listidx;
                                                    datanum++;
                                                }
                                                listidx++;
                                            }
                                        }
                                        iecs.logger.LogDebug("Report Inclusion Bitstring = " + datanum.ToString());
                                        continue;
                                    }
                                }
#if false
                                if (phase == phsDataReferences)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    //phase++;
                                    if ((rptOpts[0] & OptFldsDataReference) != 0)
                                    {
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            varName = list[i].Success.Visible_string;
                                            iecs.logger.LogDebug("Report Variable Name = " + varName);
                                            NodeBase b = iecs.DataModel.ied.FindNodeByAddress(varName);
                                            Data dataref = list[i + datanum].Success;
                                            if (!(b is NodeFC))
                                                // dataref = (dataref.Structure as List<Data>)[0];
                                                if (list[i + datanum].Success != null)
                                                    recursiveReadData(iecs, dataref, b, NodeState.Reported);
                                            datacnt++;
                                        }
                                        // Evaluation of OptFldsDataReference
                                        if (datacnt == datanum)
                                            phase += 2;   // phsReasonCodes
                                        else
                                            continue;
                                    }
                                    else
                                        phase++;
                                }
#endif
                                if (phase == phsDataReferences)
                                { // Is this phase active, e.g. is this bit set in OptFlds??
                                    //phase++;
                                    if ((rptOpts[0] & OptFldsDataReference) != 0)
                                    {
                                        if (list[i].Success.isVisible_stringSelected())
                                        {
                                            varName = list[i].Success.Visible_string;
                                            iecs.logger.LogDebug("Report Variable Name = " + varName);
                                            NodeBase b = iecs.DataModel.ied.FindNodeByAddress(varName);
                                            Data dataref = list[i + datanum].Success;
                                            if (!(b is NodeFC))
                                            {
                                                // dataref = (dataref.Structure as List<Data>)[0];
                                                if (list[i + datanum].Success != null)
                                                {
                                                    recursiveReadData(iecs, dataref, b, NodeState.Reported);

                                                    if (_env.winMgr.ReportsRunning) createReportRecord(iecs, varName, b);
                                                }
                                            }
                                            datacnt++;
                                        }
                                        // Evaluation of OptFldsDataReference
                                        if (datacnt == datanum)
                                            break;
                                        else
                                            continue;
                                    }
                                    else
                                        phase++;
                                }
                                // here we will be only if report is received without variable names
                                if (phase == phsValues)
                                {
                                    // Report WITHOUT references:
                                    // Need to investigate report members
                                    NodeBase lvb = iecs.DataModel.lists.FindNodeByAddress(datName, true);
                                    if (lvb != null)
                                    {
                                        NodeBase[] nba = lvb.GetChildNodes();

                                        if (datacnt < nba.Length)
                                        {
                                            if (!(nba[datacnt] is NodeFC))
                                            {
                                                Data dataref = list[i].Success;
                                                if (list[i].Success != null)
                                                {
                                                    recursiveReadData(iecs, dataref, nba[listmap[datacnt]], NodeState.Reported);
                                                    if (_env.winMgr.ReportsRunning)
                                                    {
                                                        varName = nba[listmap[datacnt]].CommAddress.Domain + "/" + nba[listmap[datacnt]].CommAddress.Variable;
                                                        createReportRecord(iecs, varName, nba[listmap[datacnt]]);
                                                    }
                                                }
                                            }
                                            datacnt++;
                                        }
                                        else break;
                                        // End or continue?
                                        if (datacnt == datanum)
                                            phase++;  // phsReasonCodes
                                        else
                                            continue;
                                    }
                                }
                                if (phase == phsReasonCodes)
                                {
                                    reasoncnt++;
                                    // End or continue?
                                    if (reasoncnt == datanum)
                                        break;  // phsReasonCodes
                                    else
                                        continue;
                                }
                            }
                        }
                    }
                }
            }
            else if (Report != null && Report.VariableAccessSpecification != null && Report.VariableAccessSpecification.isListOfVariableSelected())
            {
                foreach (VariableAccessSpecification.ListOfVariableSequenceType lvs in Report.VariableAccessSpecification.ListOfVariable)
                {
                    string lstErr = (lvs.VariableSpecification.isNameSelected() ? (lvs.VariableSpecification.Name.isVmd_specificSelected() ? lvs.VariableSpecification.Name.Vmd_specific.Value : "??") : "??");
                    if (lstErr == "LastApplError")
                    {
                        int phs = 0;
                        string cntrlObj = "";
                        ControlError error = ControlError.NoError;
                        OrCat originOrCat = OrCat.NOT_SUPPORTED;
                        string originOrStr = "";
                        long ctlNum = 0;
                        ControlAddCause addCause = ControlAddCause.ADD_CAUSE_UNKNOWN;

                        foreach (AccessResult ara in Report.ListOfAccessResult)
                        {
                            if (ara.isSuccessSelected() && ara.Success.isStructureSelected())
                            {
                                foreach (Data data in ara.Success.Structure)
                                {
                                    switch (phs++)
                                    {
                                        case 0:
                                            if (data.isVisible_stringSelected())
                                                cntrlObj = data.Visible_string;
                                            break;
                                        case 1:
                                            if (data.isIntegerSelected())
                                                error = (ControlError)data.Integer;
                                            break;
                                        case 2:
                                            if (data.isStructureSelected())
                                            {
                                                int j = 0;
                                                foreach (Data d in data.Structure)
                                                {
                                                    if (j == 0)
                                                    {
                                                        if (d.isIntegerSelected())
                                                            originOrCat = (OrCat)d.Integer;
                                                    }
                                                    if (j == 1)
                                                    {
                                                        if (d.isOctet_stringSelected())
                                                        {
                                                            originOrStr = System.Text.Encoding.ASCII.GetString(d.Octet_string);
                                                        }
                                                    }
                                                    ++j;
                                                }
                                            }
                                            break;
                                        case 3:
                                            if (data.isIntegerSelected())
                                                ctlNum = data.Integer;
                                            break;
                                        case 4:
                                            if (data.isIntegerSelected())
                                                addCause = (ControlAddCause)data.Integer;
                                            break;
                                    } // switch
                                }
                            } // if
                        } // foreach
                        Logger.getLogger().LogWarning("Have got LastApplError:" +
                            ", Control Object: " + cntrlObj +
                            ", Error: " + ((int)error).ToString() + " (" + Enum.GetName(typeof(ControlError), error) + ")" +
                            ", Originator: " + ((int)originOrCat).ToString() + " (" + Enum.GetName(typeof(OrCat), originOrCat) + "), Id = " + originOrStr +
                            ", CtlNum: " + ctlNum.ToString() +
                            ", addCause: " + ((int)addCause).ToString() + " (" + Enum.GetName(typeof(ControlAddCause), addCause) + ")"
                             );
                    }
                    else
                        iecs.logger.LogDebug("Have unknown Unconfirmed PDU: " + lstErr);
                }
            }
        }

        private void createReportRecord(Iec61850State iecs, string varName, NodeBase b)
        {
            /* Get information needed to log report */

            string rptdVarQuality = "";
            string rptdVarTimeQuality = "";
            StringBuilder rptdVarValue = new StringBuilder("");
            string rptdVarTimestamp = "";
            string rptdVarDescription = "";
            string rptdVarPath = "";

            NodeBase[] nb = b.GetChildNodes();

            if (nb.Length == 0)
            {
                // Probably we've got report information about single DA not DO, so we don't have new infofmation about t and q
                nb = new NodeBase[1] { b };
                varName = varName.Replace("$" + b.Name, "");
                NodeBase t = iecs.DataModel.ied.FindNodeByAddress(varName + "$t");
                if (t != null)
                    rptdVarTimestamp = (t as NodeData).StringValue;
            }

            rptdVarPath = nb[0].IecAddress;

            foreach (NodeBase nbs in nb) {
                switch (nbs.Name)
                {
                    case "general":
                        rptdVarValue.Append((nbs as NodeData).StringValue);
                        break;
                    case "dirGeneral":
                        rptdVarValue.AppendFormat("/{0}", (nbs as NodeData).StringValue);
                        break;
                    case "stVal":
                        if (nbs.IecAddress.Contains("XCBR") || nbs.IecAddress.Contains("XSWI"))
                        {
                            String vals = (nbs as NodeData).StringValue;
                            switch (vals)
                            {
                                case "01":
                                    rptdVarValue.Append("Open");
                                    break;
                                case "10":
                                    rptdVarValue.Append("Closed");
                                    break;
                                case "00":
                                case "11":
                                    rptdVarValue.Append("Bad Pos");
                                    break;
                                default:
                                    break;
                            }
                        }
                        else
                            rptdVarValue.Append((nbs as NodeData).StringValue);

                        break;
                    case "cVal":
                        if (nbs.IecAddress.Contains("MMX")) {
                            NodeData nbmag = (NodeData) nbs.GetActualChildNode();
                            if (nbmag.Name == "mag") {
                                NodeData nbf = (NodeData) nbmag.GetActualChildNode();
                                rptdVarValue.Append(nbf.DataValue);
                            }
                            
                        } else
                            rptdVarValue.Append((nbs as NodeData).StringValue);

                        break;
                    case "q":
                        rptdVarQuality = (nbs as NodeData).StringValueQuality;
                        break;
                    case "t":
                        rptdVarTimestamp = (nbs as NodeData).StringValue;
                        break;
                    default:
                        rptdVarValue.Append((nbs as NodeData).StringValue);
                        break;
                }

            }

            NodeBase d = iecs.DataModel.ied.FindNodeByAddress(varName.Replace("$ST$", "$DC$"));
            if (d != null)
            {
                NodeBase[] nd = d.GetChildNodes();

                foreach (NodeBase nds in nd)
                {
                    if (nds.Name == "d")
                        rptdVarDescription = (nds as NodeData).StringValue;
                }
            }
            else
                rptdVarDescription = "";

            rptdVarTimeQuality = rptdVarTimestamp.Contains("Bad Time Quality") ? "T" : "";

            iecs.Controller.FireNewReport(rptdVarQuality + rptdVarTimeQuality, rptdVarTimestamp, rptdVarPath, rptdVarDescription, rptdVarValue.ToString());
            //return varName;
        }

        private void ReceiveRead(Iec61850State iecs, Read_Response Read, NodeBase[] lastOperationData)
        {
            iecs.logger.LogDebug("Read != null");
            if (Read.VariableAccessSpecification != null)
            {
                iecs.logger.LogDebug("Read.VariableAccessSpecification != null");

                if (Read.VariableAccessSpecification.ListOfVariable != null)
                {
                    iecs.logger.LogDebug("Read.VariableAccessSpecification.ListOfVariable != null");
                    if (Read.ListOfAccessResult != null)
                    {
                        if (Read.ListOfAccessResult.Count == Read.VariableAccessSpecification.ListOfVariable.Count)
                        {
                            IEnumerator<AccessResult> are = Read.ListOfAccessResult.GetEnumerator();
                            IEnumerator<VariableAccessSpecification.ListOfVariableSequenceType> vase = Read.VariableAccessSpecification.ListOfVariable.GetEnumerator();
                            while (are.MoveNext() && vase.MoveNext())
                            {
                                iecs.logger.LogDebug("Reading variable: " + vase.Current.VariableSpecification.Name.Domain_specific.ItemID.Value);
                                NodeBase b = (iecs.DataModel.ied as NodeIed).FindNodeByAddress(vase.Current.VariableSpecification.Name.Domain_specific.DomainID.Value, vase.Current.VariableSpecification.Name.Domain_specific.ItemID.Value);
                                if (b != null)
                                {
                                    iecs.logger.LogDebug("Node address: " + b.IecAddress);
                                    recursiveReadData(iecs, are.Current.Success, b, NodeState.Read);
                                }
                            }
                        }
                        else
                        {
                            // Error
                            iecs.logger.LogError("Error reading variables, different count of Specifications and AccessResults!");
                        }
                    }
                }
                else if (Read.VariableAccessSpecification.VariableListName != null)
                {
                    // reading a NVL read
                    iecs.logger.LogDebug("Read.VariableAccessSpecification.ListOfVariable != null");
                    if (Read.ListOfAccessResult != null)
                    {
                        // Find the NVL by name
                        if (Read.VariableAccessSpecification.VariableListName.Domain_specific != null)
                        {
                            string domain = Read.VariableAccessSpecification.VariableListName.Domain_specific.DomainID.Value;
                            string address = Read.VariableAccessSpecification.VariableListName.Domain_specific.ItemID.Value;
                            NodeBase nb = iecs.DataModel.lists.FindNodeByAddress(domain, address, true);
                            // TODO
                            if (nb != null && nb.GetChildCount() == Read.ListOfAccessResult.Count)
                            {
                                NodeBase[] data = nb.GetChildNodes();

                                for (int i = 0; i < data.Length; i++)
                                {
                                    iecs.logger.LogDebug("Reading variable: " + data[i].IecAddress);
                                    recursiveReadData(iecs, (Read.ListOfAccessResult as List<AccessResult>)[i].Success, data[i], NodeState.Read);
                                }
                            }
                        }
                    }
                }
            }
            else    // When VariableAccessSpecification is missing, try to read to actual variable
            {
                if (Read.ListOfAccessResult != null)
                {
                    int i = 0;
                    // libiec61850 correction
                    // one read of a node is equal to separate reads to node children
                    if (Read.ListOfAccessResult.Count > lastOperationData.Length)
                    {
                        if (Read.ListOfAccessResult.Count == lastOperationData[0].GetChildNodes().Length)
                        {
                            lastOperationData = lastOperationData[0].GetChildNodes();
                        }
                    }
                    // libiec61850 correction end
                    foreach (AccessResult ar in Read.ListOfAccessResult)
                    {
                        if (i <= lastOperationData.GetUpperBound(0))
                        {
                            if (ar.Success != null)
                            {
                                iecs.logger.LogDebug("Reading Actual variable value: " + lastOperationData[i].IecAddress);
                                recursiveReadData(iecs, ar.Success, lastOperationData[i], NodeState.Read);
                            }
                        }
                        else
                            iecs.logger.LogError("Not matching read structure in ReceiveRead");
                        i++;
                    }
                    lastOperationData = null;
                }
            }
            if (iecs.istate == Iec61850lStateEnum.IEC61850_READ_MODEL_DATA_WAIT)
            {
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                if (iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                    {
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            // End of loop
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                            iecs.logger.LogInfo("Reading named variable lists: [IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST]");
                            iecs.DataModel.ied.ResetAllChildNodes();
                        }
                    }
                }
            }
        }

        private void ReceiveGetNamedVariableListAttributes(Iec61850State iecs, GetNamedVariableListAttributes_Response GetNamedVariableListAttributes)
        {
            iecs.logger.LogDebug("GetNamedVariableListAttributes != null");
            if (GetNamedVariableListAttributes.MmsDeletable)
                (iecs.DataModel.lists.GetActualChildNode().GetActualChildNode() as NodeVL).Deletable = true;

            if (GetNamedVariableListAttributes.ListOfVariable != null)
            {
                iecs.logger.LogDebug("GetVariableAccessAttributes.ListOfVariable != null");
                foreach (GetNamedVariableListAttributes_Response.ListOfVariableSequenceType v in GetNamedVariableListAttributes.ListOfVariable)
                {
                    iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}/{1}", v.VariableSpecification.Name.Domain_specific.DomainID.Value, v.VariableSpecification.Name.Domain_specific.ItemID.Value));
                    NodeBase b = (iecs.DataModel.ied as NodeIed).FindNodeByAddress(v.VariableSpecification.Name.Domain_specific.DomainID.Value, v.VariableSpecification.Name.Domain_specific.ItemID.Value);
                    if (b != null)
                    {
                        iecs.DataModel.lists.GetActualChildNode().GetActualChildNode().LinkChildNodeByAddress(b);
                    }
                }
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST;
                if (iecs.DataModel.lists.GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.lists.NextActualChildNode() == null)
                    {
                        // End of loop
                        iecs.istate = Iec61850lStateEnum.IEC61850_MAKEGUI;
                        iecs.logger.LogInfo("Init end: [IEC61850_FREILAUF]");
                        iecs.DataModel.lists.ResetAllChildNodes();
                        //iecs.
                    }
                }
            }
        }

        private void ReceiveGetVariableAccessAttributes(Iec61850State iecs, GetVariableAccessAttributes_Response GetVariableAccessAttributes)
        {
            iecs.logger.LogDebug("GetVariableAccessAttributes != null");
            if (GetVariableAccessAttributes.TypeDescription != null)
            {
                iecs.logger.LogDebug("GetVariableAccessAttributes.TypeDescription != null");
                RecursiveReadTypeDescription(iecs, iecs.DataModel.ied.GetActualChildNode().GetActualChildNode(),
                                             GetVariableAccessAttributes.TypeDescription);
                iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;
                if (iecs.DataModel.ied.GetActualChildNode().NextActualChildNode() == null)
                {
                    if (iecs.DataModel.ied.NextActualChildNode() == null)
                    {
                        // End of loop
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_MODEL_DATA;
                        iecs.logger.LogInfo("Reading variable values: [IEC61850_READ_MODEL_DATA]");
                        iecs.DataModel.ied.ResetAllChildNodes();
                    }
                }
            }
        }

        private void ReceiveGetNameList(Iec61850State iecs, GetNameList_Response GetNameList)
        {
            switch (iecs.istate)
            {
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN_WAIT:
                    foreach (Identifier i in GetNameList.ListOfIdentifier)
                    {
                        iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}", i.Value));
                        iecs.DataModel.ied.AddChildNode(new NodeLD(i.Value));
                        iecs.continueAfter = null;
                    }
                    iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;
                    iecs.logger.LogInfo("Reading variable names: [IEC61850_READ_NAMELIST_VAR]");
                    break;
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR_WAIT:
                    foreach (Identifier i in GetNameList.ListOfIdentifier)
                    {
                        iecs.logger.LogDebug("GetNameList.ListOfIdentifier: " + i.Value.ToString());
                        AddIecAddress(iecs, i.Value);
                        iecs.continueAfter = i;
                    }
                    iecs.logger.LogDebug("GetNameList.MoreFollows: " + GetNameList.MoreFollows.ToString());
                    if (GetNameList.MoreFollows)
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;
                    else
                    {
                        iecs.continueAfter = null;
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_VAR;    // next state
                            iecs.logger.LogInfo("Reading variable specifications: [IEC61850_READ_ACCESSAT_VAR]");
                            iecs.DataModel.ied.ResetAllChildNodes();
                        }
                        else
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_VAR;         // next logical device
                    }
                    break;
                case Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST_WAIT:
                    if (GetNameList.ListOfIdentifier != null)
                        foreach (Identifier i in GetNameList.ListOfIdentifier)
                        {
                            iecs.logger.LogDebug(String.Format("GetNameList.ListOfIdentifier: {0}", i.Value));
                            NodeBase nld = iecs.DataModel.lists.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            NodeVL vl = new NodeVL(i.Value);
                            vl.Defined = true;
                            nld.AddChildNode(vl);
                            iecs.continueAfter = null;
                        }
                    if (GetNameList.MoreFollows)
                        iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;
                    else
                    {
                        iecs.continueAfter = null;
                        if (iecs.DataModel.ied.NextActualChildNode() == null)
                        {
                            iecs.logger.LogInfo("Reading variable lists attributes: [IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST]");    // next state
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_ACCESSAT_NAMED_VARIABLE_LIST; // next state
                            iecs.DataModel.ied.ResetAllChildNodes();
                            iecs.DataModel.lists.ResetAllChildNodes();
                        }
                        else
                            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_NAMED_VARIABLE_LIST;         // next logical device
                    }
                    break;
            }
        }

        private void ReceiveIdentify(Iec61850State iecs, Identify_Response Identify)
        {
            iecs.logger.LogInfo(String.Format("Received Identify: {0}, {1}, {2}",
                Identify.VendorName.Value,
                Identify.ModelName.Value,
                Identify.Revision.Value
                ));
            (iecs.DataModel.ied as NodeIed).VendorName = Identify.VendorName.Value;
            (iecs.DataModel.ied as NodeIed).ModelName = Identify.ModelName.Value;
            (iecs.DataModel.ied as NodeIed).Revision = Identify.Revision.Value;
            iecs.istate = Iec61850lStateEnum.IEC61850_READ_NAMELIST_DOMAIN;
            iecs.logger.LogInfo("Reading domain (LD) names: [IEC61850_READ_NAMELIST_DOMAIN]");
        }

        void recursiveReadData(Iec61850State iecs, Data data, NodeBase actualNode, NodeState s)
        {
            if (data == null)
            {
                iecs.logger.LogDebug("data = null, returning from recursiveReadData");
                return;
            }
            if (actualNode == null)
            {
                iecs.logger.LogDebug("actualNode = null, returning from recursiveReadData");
                return;
            }
            iecs.logger.LogDebug("recursiveReadData: nodeAddress=" + actualNode.IecAddress + ", state=" + s.ToString());
            actualNode.NodeState = s;
            if (data.Structure != null)
            {
                iecs.logger.LogDebug("data.Structure != null");
                NodeBase[] nb = actualNode.GetChildNodes();
                int i = 0;
                foreach (Data d in data.Structure)
                {
                    if (i <= nb.GetUpperBound(0))
                        recursiveReadData(iecs, d, nb[i], s);
                    else
                        iecs.logger.LogError("Not matching read structure: Node=" + actualNode.IecAddress);
                    i++;
                }
            }
            else if (data.Array != null)
            {
                iecs.logger.LogDebug("data.Array != null");
                int i = 0;
                NodeBase[] nb = actualNode.GetChildNodes();
                foreach (Data d in data.Array)
                {
                    if (i <= nb.GetUpperBound(0))
                        recursiveReadData(iecs, d, nb[i], s);
                    else
                        iecs.logger.LogError("Not matching read array: Node=" + actualNode.Name);
                    i++;
                }
            }
            else if (data.isIntegerSelected())
            {
                iecs.logger.LogDebug("data.Integer != null");
                (actualNode as NodeData).DataValue = data.Integer;
            }
            else if (data.isBcdSelected())
            {
                iecs.logger.LogDebug("data.Bcd != null");
                (actualNode as NodeData).DataValue = data.Bcd;
            }
            else if (data.isBooleanSelected())
            {
                iecs.logger.LogDebug("data.Boolean != null");
                (actualNode as NodeData).DataValue = data.Boolean;
            }
            else if (data.isFloating_pointSelected())
            {
                iecs.logger.LogDebug("data.Floating_point != null");
                if (data.Floating_point.Value.Length == 5)
                {
                    float k = 0.0F;
                    byte[] tmp = new byte[4];
                    tmp[0] = data.Floating_point.Value[4];
                    tmp[1] = data.Floating_point.Value[3];
                    tmp[2] = data.Floating_point.Value[2];
                    tmp[3] = data.Floating_point.Value[1];
                    k = BitConverter.ToSingle(tmp, 0);
                    (actualNode as NodeData).DataValue = k;
                }
                else if (data.Floating_point.Value.Length == 9)
                {
                    double k = 0.0;
                    byte[] tmp = new byte[8];
                    tmp[0] = data.Floating_point.Value[8];
                    tmp[1] = data.Floating_point.Value[7];
                    tmp[2] = data.Floating_point.Value[6];
                    tmp[3] = data.Floating_point.Value[5];
                    tmp[4] = data.Floating_point.Value[4];
                    tmp[5] = data.Floating_point.Value[3];
                    tmp[6] = data.Floating_point.Value[2];
                    tmp[7] = data.Floating_point.Value[1];
                    k = BitConverter.ToDouble(tmp, 0);
                    (actualNode as NodeData).DataValue = k;
                }
            }
            else if (data.isGeneralized_timeSelected())
            {
                iecs.logger.LogDebug("data.Generalized_time != null");
                (actualNode as NodeData).DataValue = data.Generalized_time;
            }
            else if (data.isMMSStringSelected())
            {
                iecs.logger.LogDebug("data.MMSString != null");
                (actualNode as NodeData).DataValue = data.MMSString.Value;
            }
            else if (data.isObjIdSelected())
            {
                iecs.logger.LogDebug("data.ObjId != null");
                (actualNode as NodeData).DataValue = data.ObjId.Value;
            }
            else if (data.isOctet_stringSelected())
            {
                iecs.logger.LogDebug("data.Octet_string != null");
                (actualNode as NodeData).DataValue = (data.Octet_string);
            }
            else if (data.isUnsignedSelected())
            {
                iecs.logger.LogDebug("data.Unsigned != null");
                (actualNode as NodeData).DataValue = data.Unsigned;
            }
            else if (data.isUtc_timeSelected())
            {
                iecs.logger.LogDebug("data.Utc_time != null");

                (actualNode as NodeData).DataValue = ConvertFromUtcTime(data.Utc_time.Value, (actualNode as NodeData).DataParam);
            }
            else if (data.isVisible_stringSelected())
            {
                iecs.logger.LogDebug("data.Visible_string != null");
                (actualNode as NodeData).DataValue = data.Visible_string;
            }
            else if (data.isBinary_timeSelected())
            {
                iecs.logger.LogDebug("data.Binary_time != null");

                ulong millis;
                ulong days = 0;
                DateTime origin;

                millis = (ulong)(data.Binary_time.Value[0] << 24) +
                         (ulong)(data.Binary_time.Value[1] << 16) +
                         (ulong)(data.Binary_time.Value[2] << 8) +
                         (ulong)(data.Binary_time.Value[3]);
                if (data.Binary_time.Value.Length == 6)
                {
                    days = (ulong)(data.Binary_time.Value[4] << 8) +
                           (ulong)(data.Binary_time.Value[5]);
                    origin = new DateTime(1984, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
                    //millis *= 1000;
                }
                else
                {
                    origin = DateTime.UtcNow.Date;
                }

                double dMillis = (double)(millis + days * 24 * 3600 * 1000);
                origin = origin.AddMilliseconds(dMillis);

                (actualNode as NodeData).DataValue = origin.ToLocalTime();
                (actualNode as NodeData).DataParam = data.Binary_time.Value;

            }
            else if (data.isBit_stringSelected())
            {
                iecs.logger.LogDebug("data.Bit_string != null");
                (actualNode as NodeData).DataValue = data.Bit_string.Value;
                (actualNode as NodeData).DataParam = data.Bit_string.TrailBitsCnt;
            }
            iecs.logger.LogDebug("recursiveReadData: successfull return");
        }

        void RecursiveReadTypeDescription(Iec61850State iecs, NodeBase actualNode, TypeDescription t)
        {
            if (t == null) return;
            if (t.Structure != null)
            {
                iecs.logger.LogDebug("t.Structure != null");
                if (t.Structure.Components != null) foreach (TypeDescription.StructureSequenceType.ComponentsSequenceType s in t.Structure.Components)
                    {
                        iecs.logger.LogDebug(s.ComponentName.Value);
                        NodeBase newActualNode;
                        // DO or DA?
                        bool isDO = false;
                        if (actualNode is NodeFC) isDO = true;  // Safe to say under FC must be a DO
                        if (isDO)
                            newActualNode = new NodeDO(s.ComponentName.Value);
                        else
                            newActualNode = new NodeData(s.ComponentName.Value);
                        newActualNode = actualNode.AddChildNode(newActualNode);
                        RecursiveReadTypeDescription(iecs, newActualNode, s.ComponentType.TypeDescription);
                        if (actualNode is NodeFC && (actualNode.Name == "RP" || actualNode.Name == "BR"))
                        {
                            // Having RCB
                            NodeBase nrpied;
                            if (actualNode.Name == "RP") nrpied = iecs.DataModel.urcbs.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            else nrpied = iecs.DataModel.brcbs.AddChildNode(new NodeLD(iecs.DataModel.ied.GetActualChildNode().Name));
                            NodeBase nrp = new NodeRCB(newActualNode.CommAddress.Variable);
                            nrpied.AddChildNode(nrp);
                            foreach (NodeBase nb in newActualNode.GetChildNodes())
                            {
                                nrp.LinkChildNodeByAddress(nb);
                            }
                        }
                    }
            }
            else if (t.Array != null)
            {
                iecs.logger.LogDebug("t.Array != null");
                if (actualNode is NodeData && !(actualNode is NodeDO))
                    (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.array;
                for (int i = 0; i < t.Array.NumberOfElements.Value; i++)
                {
                    NodeData newActualNode = new NodeData("[" + i.ToString() + "]");
                    actualNode.AddChildNode(newActualNode);
                    RecursiveReadTypeDescription(iecs, newActualNode, t.Array.ElementType.TypeDescription);
                }
            }
            else if (t.Integer != null)
            {
                iecs.logger.LogDebug("t.Integer != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.integer;
            }
            else if (t.Bcd != null)
            {
                iecs.logger.LogDebug("t.Bcd != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.bcd;
            }
            else if (t.Boolean != null)
            {
                iecs.logger.LogDebug("t.Boolean != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.boolean;
            }
            else if (t.Floating_point != null)
            {
                iecs.logger.LogDebug("t.Floating_point != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.floating_point;
            }
            else if (t.Generalized_time != null)
            {
                iecs.logger.LogDebug("t.Generalized_time != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.generalized_time;
            }
            else if (t.MMSString != null)
            {
                iecs.logger.LogDebug("t.MMSString != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.mMSString;
            }
            else if (t.ObjId != null)
            {
                iecs.logger.LogDebug("t.ObjId != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.objId;
            }
            else if (t.Octet_string != null)
            {
                iecs.logger.LogDebug("t.Octet_string != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.octet_string;
            }
            else if (t.Unsigned != null)
            {
                iecs.logger.LogDebug("t.Unsigned != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.unsigned;
            }
            else if (t.Utc_time != null)
            {
                iecs.logger.LogDebug("t.Utc_time != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.utc_time;
            }
            else if (t.Visible_string != null)
            {
                iecs.logger.LogDebug("t.Visible_string != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.visible_string;
            }
            else if (t.isBinary_timeSelected())
            {
                iecs.logger.LogDebug("t.Binary_time != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.binary_time;
            }
            else if (t.isBit_stringSelected())
            {
                iecs.logger.LogDebug("t.Bit_string != null");
                (actualNode as NodeData).DataType = scsm_MMS_TypeEnum.bit_string;
            }
        }

        public int ReceiveInitiate(Iec61850State iecs, Initiate_ResponsePDU initiate_ResponsePDU)
        {
            if (initiate_ResponsePDU != null)
            {
                iecs.logger.LogDebug("mymmspdu.Initiate_ResponsePDU exists!");
                int cing = initiate_ResponsePDU.NegotiatedMaxServOutstandingCalling.Value;
                int ced = initiate_ResponsePDU.NegotiatedMaxServOutstandingCalled.Value;
                iecs.logger.LogDebug(String.Format("mymmspdu.Initiate_ResponsePDU.NegotiatedMaxServOutstandingCalling: {0}, Called: {1}",
                    cing, ced));

                MaxCalls = cing < ced ? cing : ced;

                StringBuilder sb2 = new StringBuilder();
                int j = 0;
                foreach (byte b in initiate_ResponsePDU.InitResponseDetail.ServicesSupportedCalled.Value.Value)
                {
                    for (int i = 7; i >= 0; i--)
                    {
                        ServiceSupportOptions[j] = ((b >> i) & 1) == 1;
                        if (ServiceSupportOptions[j]) sb2.Append(Enum.GetName(typeof(ServiceSupportOptionsEnum), (ServiceSupportOptionsEnum)j) + ',');
                        j++;
                    }
                }
                iecs.logger.LogInfo("Services Supported: " + sb2.ToString());

                if (ServiceSupportOptions[(int)ServiceSupportOptionsEnum.defineNamedVariableList])
                {
                    iecs.DataModel.ied.DefineNVL = true;
                }
                if (ServiceSupportOptions[(int)ServiceSupportOptionsEnum.identify])
                {
                    iecs.DataModel.ied.Identify = true;
                }
            }
            else
            {
                iecs.logger.LogError("mms.ReceiveInitiate: not an Initiate_ResponsePDU");
                return -1;
            }
            return 0;
        }


        /*///////////////////////////////////////////////////////////////////////////////////////////////////////////////////*/

        public int SendIdentify(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Identify_Request idreq = new Identify_Request();

            idreq.initWithDefaults();

            csrreq.selectIdentify(idreq);

            InvokeID = 0;
            crreq.InvokeID = new Unsigned32(InvokeID++);
            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendIdentify: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendGetNameListDomain(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_domain);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            nlreq.ObjectScope.selectVmdSpecific();

            csrreq.selectGetNameList(nlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendGetNameListDomain: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendGetNameListVariables(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_namedVariable);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            nlreq.ObjectScope.selectDomainSpecific(new Identifier(iecs.DataModel.ied.GetActualChildNode().Name));
            nlreq.ContinueAfter = iecs.continueAfter;

            csrreq.selectGetNameList(nlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendGetNameListVariables: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendGetNameListNamedVariableList(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNameList_Request nlreq = new GetNameList_Request();

            nlreq.ObjectClass = new ObjectClass();
            nlreq.ObjectClass.selectBasicObjectClass(ObjectClass.ObjectClass__basicObjectClass_namedVariableList);
            nlreq.ObjectScope = new GetNameList_Request.ObjectScopeChoiceType();
            nlreq.ObjectScope.selectDomainSpecific(new Identifier(iecs.DataModel.ied.GetActualChildNode().Name));
            nlreq.ContinueAfter = iecs.continueAfter;

            csrreq.selectGetNameList(nlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendGetNameListVariables: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendGetVariableAccessAttributes(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetVariableAccessAttributes_Request vareq = new GetVariableAccessAttributes_Request();
            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            dst.DomainID = new Identifier(iecs.DataModel.ied.GetActualChildNode().Name);
            dst.ItemID = new Identifier(iecs.DataModel.ied.GetActualChildNode().GetActualChildNode().Name);         // LN name e.g. MMXU0

            iecs.logger.LogDebug("SendGetVariableAccessAttributes: Get Attr for: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            vareq.selectName(on);

            csrreq.selectGetVariableAccessAttributes(vareq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendGetVariableAccessAttributes: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendGetNamedVariableListAttributes(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            GetNamedVariableListAttributes_Request nareq = new GetNamedVariableListAttributes_Request();
            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            NodeBase n = iecs.DataModel.lists.GetActualChildNode();
            if (n == null)
            {
                iecs.logger.LogDebug("mms.SendGetNamedVariableListAttributes: No lists defined!");
                return -2;
            }

            dst.DomainID = new Identifier(n.Name);
            dst.ItemID = new Identifier(iecs.DataModel.lists.GetActualChildNode().GetActualChildNode().Name);         // List name e.g. MMXU0$MX

            iecs.logger.LogDebug("GetNamedVariableListAttributes: Get Attr for: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            nareq.Value = on;

            csrreq.selectGetNamedVariableListAttributes(nareq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendGetNamedVariableListAttributes: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, null);

            return 0;
        }

        public int SendRead(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Read_Request rreq = new Read_Request();

            List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();

            foreach (NodeBase b in el.Data)
            {
                VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();

                ObjectName on = new ObjectName();
                ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

                vas.VariableSpecification = new VariableSpecification();
                vas.VariableSpecification.selectName(on);

                dst.DomainID = new Identifier(b.CommAddress.Domain);
                dst.ItemID = new Identifier(b.CommAddress.Variable);

                iecs.logger.LogDebug("SendRead: Reading: " + dst.ItemID.Value);
                on.selectDomain_specific(dst);

                vasl.Add(vas);
            }

            //iecs.lastOperationData = el.Data;

            rreq.VariableAccessSpecification = new VariableAccessSpecification();
            rreq.VariableAccessSpecification.selectListOfVariable(vasl);
            rreq.SpecificationWithResult = true;

            csrreq.selectRead(rreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendRead: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);

            return 0;
        }

        public int SendReadVL(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Read_Request rreq = new Read_Request();

            NodeBase b = el.Data[0];    // Must be NodeVL

            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();

            dst.DomainID = new Identifier(b.CommAddress.Domain);
            dst.ItemID = new Identifier(b.CommAddress.Variable);
            iecs.logger.LogDebug("SendRead: Reading with NVL: " + dst.ItemID.Value);
            on.selectDomain_specific(dst);

            rreq.VariableAccessSpecification = new VariableAccessSpecification();
            rreq.VariableAccessSpecification.selectVariableListName(on);
            rreq.SpecificationWithResult = true;

            csrreq.selectRead(rreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendRead: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);

            return 0;
        }

        public int SendWrite(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Write_Request wreq = new Write_Request();

            List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
            List<Data> datl = new List<Data>();

            foreach (NodeData d in el.Data)
            {
                if (d != null)
                {
                    VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
                    Data dat = new Data();

                    ObjectName on = new ObjectName();
                    ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
                    dst.DomainID = new Identifier(el.Address.Domain);
                    dst.ItemID = new Identifier(el.Address.Variable + "$" + d.Name);
                    on.selectDomain_specific(dst);

                    vas.VariableSpecification = new VariableSpecification();
                    vas.VariableSpecification.selectName(on);

                    vasl.Add(vas);

                    switch (d.DataType)
                    {
                        case scsm_MMS_TypeEnum.boolean:
                            dat.selectBoolean((bool)d.DataValue);
                            break;
                        case scsm_MMS_TypeEnum.visible_string:
                            dat.selectVisible_string((string)d.DataValue);
                            break;
                        case scsm_MMS_TypeEnum.octet_string:
                            dat.selectOctet_string((byte[])d.DataValue);
                            break;
                        case scsm_MMS_TypeEnum.utc_time:
                            UtcTime val = new UtcTime((byte[])d.DataValue);
                            dat.selectUtc_time(val);
                            break;
                        case scsm_MMS_TypeEnum.bit_string:
                            dat.selectBit_string(new BitString((byte[])d.DataValue, (int)d.DataParam));
                            break;
                        case scsm_MMS_TypeEnum.unsigned:
                            dat.selectUnsigned((long)d.DataValue);
                            break;
                        case scsm_MMS_TypeEnum.integer:
                            dat.selectInteger((long)d.DataValue);
                            break;
                        case scsm_MMS_TypeEnum.floating_point:
                            byte[] byteval;
                            byte[] tmp;
                            if (d.DataValue is float)
                            {
                                byteval = new byte[5];
                                tmp = BitConverter.GetBytes((float)d.DataValue);
                                byteval[4] = tmp[0];
                                byteval[3] = tmp[1];
                                byteval[2] = tmp[2];
                                byteval[1] = tmp[3];
                                byteval[0] = 0x08;
                            }
                            else
                            {
                                byteval = new byte[9];
                                tmp = BitConverter.GetBytes((float)d.DataValue);
                                byteval[8] = tmp[0];
                                byteval[7] = tmp[1];
                                byteval[6] = tmp[2];
                                byteval[5] = tmp[3];
                                byteval[4] = tmp[4];
                                byteval[3] = tmp[5];
                                byteval[2] = tmp[6];
                                byteval[1] = tmp[7];
                                byteval[0] = 0x08;      // ???????????? TEST
                            }
                            FloatingPoint fpval = new FloatingPoint(byteval);
                            dat.selectFloating_point(fpval);
                            break;
                        default:
                            iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
                            return 1;
                    }
                    datl.Add(dat);

                    iecs.logger.LogDebug("SendWrite: Writing: " + dst.ItemID.Value);
                }
                else
                    iecs.logger.LogWarning("SendWrite: Null in data for write for: " + el.Address.Variable);
            }
            wreq.VariableAccessSpecification = new VariableAccessSpecification();
            wreq.VariableAccessSpecification.selectListOfVariable(vasl);
            wreq.ListOfData = datl;

            csrreq.selectWrite(wreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendWrite: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendWriteAsStructure(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            Write_Request wreq = new Write_Request();

            List<VariableAccessSpecification.ListOfVariableSequenceType> vasl = new List<VariableAccessSpecification.ListOfVariableSequenceType>();
            List<Data> datList_Seq = new List<Data>();
            List<Data> datList_Struct = new List<Data>();

            VariableAccessSpecification.ListOfVariableSequenceType vas = new VariableAccessSpecification.ListOfVariableSequenceType();
            Data dat_Seq = new Data();
            ObjectName on = new ObjectName();
            ObjectName.Domain_specificSequenceType dst = new ObjectName.Domain_specificSequenceType();
            dst.DomainID = new Identifier(el.Address.Domain);
            dst.ItemID = new Identifier(el.Address.Variable);   // until Oper
            on.selectDomain_specific(dst);
            vas.VariableSpecification = new VariableSpecification();
            vas.VariableSpecification.selectName(on);
            vasl.Add(vas);

            MakeStruct(iecs, el.Data, datList_Struct);
            iecs.logger.LogDebug("SendWrite: Writing Command Structure: " + dst.ItemID.Value);

            dat_Seq.selectStructure(datList_Struct);
            datList_Seq.Add(dat_Seq);

            wreq.VariableAccessSpecification = new VariableAccessSpecification();
            wreq.VariableAccessSpecification.selectListOfVariable(vasl);
            wreq.ListOfData = datList_Seq;

            csrreq.selectWrite(wreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendWriteAsStructure: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        private static void MakeStruct(Iec61850State iecs, NodeBase[] data, List<Data> datList_Struct)
        {
            foreach (NodeData d in data)
            {
                Data dat_Struct = new Data();

                switch (d.DataType)
                {
                    case scsm_MMS_TypeEnum.boolean:
                        dat_Struct.selectBoolean((bool)d.DataValue);
                        break;
                    case scsm_MMS_TypeEnum.visible_string:
                        dat_Struct.selectVisible_string((string)d.DataValue);
                        break;
                    case scsm_MMS_TypeEnum.octet_string:
                        dat_Struct.selectOctet_string((byte[])d.DataValue);
                        break;
                    case scsm_MMS_TypeEnum.utc_time:
                        UtcTime val = new UtcTime((byte[])d.DataValue);
                        dat_Struct.selectUtc_time(val);
                        break;
                    case scsm_MMS_TypeEnum.bit_string:
                        dat_Struct.selectBit_string(new BitString((byte[])d.DataValue, (int)d.DataParam));
                        break;
                    case scsm_MMS_TypeEnum.unsigned:
                        dat_Struct.selectUnsigned((long)d.DataValue);
                        break;
                    case scsm_MMS_TypeEnum.integer:
                        dat_Struct.selectInteger((long)d.DataValue);
                        break;
                    case scsm_MMS_TypeEnum.structure:
                        List<Data> datList_Struct2 = new List<Data>();
                        MakeStruct(iecs, d.GetChildNodes(), datList_Struct2);          // Recursive call
                        dat_Struct.selectStructure(datList_Struct2);
                        break;
                    default:
                        iecs.logger.LogError("mms.SendWrite: Cannot send unknown datatype!");
                        //return 1;
                        break;
                }
                datList_Struct.Add(dat_Struct);

            }
        }

        public int SendDefineNVL(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            DefineNamedVariableList_Request nvlreq = new DefineNamedVariableList_Request();
            List<DefineNamedVariableList_Request.ListOfVariableSequenceType> dnvl = new List<DefineNamedVariableList_Request.ListOfVariableSequenceType>();
            DefineNamedVariableList_Request.ListOfVariableSequenceType var;

            foreach (NodeBase d in el.Data)
            {
                var = new DefineNamedVariableList_Request.ListOfVariableSequenceType();
                var.VariableSpecification = new VariableSpecification();
                var.VariableSpecification.selectName(new ObjectName());
                var.VariableSpecification.Name.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
                var.VariableSpecification.Name.Domain_specific.DomainID = new Identifier(d.CommAddress.Domain);
                var.VariableSpecification.Name.Domain_specific.ItemID = new Identifier(d.CommAddress.Variable);
                dnvl.Add(var);
            }

            NodeBase[] nvl = new NodeBase[1];
            nvl[0] = el.Address.owner;
            //iecs.lastOperationData = nvl;

            nvlreq.ListOfVariable = dnvl;
            nvlreq.VariableListName = new ObjectName();
            nvlreq.VariableListName.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
            nvlreq.VariableListName.Domain_specific.DomainID = new Identifier(el.Address.Domain);
            nvlreq.VariableListName.Domain_specific.ItemID = new Identifier(el.Address.Variable);

            csrreq.selectDefineNamedVariableList(nvlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendDefineNVL: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, nvl);
            return 0;
        }

        public int SendDeleteNVL(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            DeleteNamedVariableList_Request dnvlreq = new DeleteNamedVariableList_Request();
            List<ObjectName> onl = new List<ObjectName>();
            ObjectName on;

            foreach (NodeBase d in el.Data)
            {
                on = new ObjectName();
                on.selectDomain_specific(new ObjectName.Domain_specificSequenceType());
                on.Domain_specific.DomainID = new Identifier(d.CommAddress.Domain);
                on.Domain_specific.ItemID = new Identifier(d.CommAddress.Variable);
                onl.Add(on);
            }

            dnvlreq.ListOfVariableListName = onl;
            //dnvlreq.DomainName = new Identifier(el.Address.Domain);
            dnvlreq.ScopeOfDelete = DeleteNamedVariableList_Request.scopeOfDelete_specific;

            csrreq.selectDeleteNamedVariableList(dnvlreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendDeleteNVL: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendFileDirectory(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileDirectory_Request filedreq = new FileDirectory_Request();
            FileName filename = new FileName();
            FileName conafter = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
                filename.Add((el.Data[0] as NodeFile).FullName);
            else
                filename.Add(el.Address.Variable);
            filedreq.FileSpecification = filename;
            if (iecs.continueAfterFileDirectory != null && iecs.continueAfterFileDirectory.Value.Count == 1)
            {
                conafter.initValue();
                string[] name = new string[1];
                iecs.continueAfterFileDirectory.Value.CopyTo(name, 0);
                conafter.Add(name[0]);
                filedreq.ContinueAfter = conafter;
            }

            iecs.lastFileOperationData = el.Data;

            csrreq.selectFileDirectory(filedreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendFileDirectory: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendFileOpen(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileOpen_Request fileoreq = new FileOpen_Request();
            FileName filename = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
                filename.Add((el.Data[0] as NodeFile).FullName);
            else
            {
                iecs.logger.LogError("mms.SendFileOpen: Request not a file!");
                return -1;
            }
            fileoreq.FileName = filename;
            fileoreq.InitialPosition = new Unsigned32(0);

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileOpen(fileoreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendFileOpen: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendFileRead(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileRead_Request filerreq = new FileRead_Request();
            if (el.Data[0] is NodeFile)
                filerreq.Value = new Integer32((el.Data[0] as NodeFile).frsmId);
            else
            {
                iecs.logger.LogError("mms.SendReadFile: Request not a file!");
                return -1;
            }

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileRead(filerreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendFileRead: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendFileDelete(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileDelete_Request filedreq = new FileDelete_Request();
            FileName filename = new FileName();

            filename.initValue();
            if (el.Data[0] is NodeFile)
                filename.Add((el.Data[0] as NodeFile).FullName);
            else
            {
                iecs.logger.LogError("mms.SendDeleteFile: Request not a file!");
                return -1;
            }
            filedreq.Value = filename;

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileDelete(filedreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendDeleteFile: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendFileClose(Iec61850State iecs, WriteQueueElement el)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Confirmed_RequestPDU crreq = new Confirmed_RequestPDU();
            ConfirmedServiceRequest csrreq = new ConfirmedServiceRequest();
            FileClose_Request filecreq = new FileClose_Request();

            if (el.Data[0] is NodeFile)
                filecreq.Value = new Integer32((el.Data[0] as NodeFile).frsmId);
            else
            {
                iecs.logger.LogError("mms.SendCloseFile: Request not a file!");
                return -1;
            }

            iecs.lastFileOperationData[0] = el.Data[0];

            csrreq.selectFileClose(filecreq);

            crreq.InvokeID = new Unsigned32(InvokeID++);

            crreq.Service = csrreq;

            mymmspdu.selectConfirmed_RequestPDU(crreq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendCloseFile: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, InvokeID, el.Data);
            return 0;
        }

        public int SendInitiate(Iec61850State iecs)
        {
            MMSpdu mymmspdu = new MMSpdu();
            iecs.msMMSout = new MemoryStream();

            Initiate_RequestPDU ireq = new Initiate_RequestPDU();
            Initiate_RequestPDU.InitRequestDetailSequenceType idet = new Initiate_RequestPDU.InitRequestDetailSequenceType();

            idet.ProposedVersionNumber = new Integer16(1);
            byte[] ppc = { 0xf1, 0x00 };
            idet.ProposedParameterCBB = new ParameterSupportOptions(new BitString(ppc, 5));

            byte[] ssc = { 0xee, 0x1c, 0x00, 0x00, 0x04, 0x08, 0x00, 0x00, 0x79, 0xef, 0x18 };
            idet.ServicesSupportedCalling = new MMS_ASN1_Model.ServiceSupportOptions(new BitString(ssc, 3));

            ireq.InitRequestDetail = idet;

            ireq.LocalDetailCalling = new Integer32(65000);
            ireq.ProposedMaxServOutstandingCalling = new Integer16(10);
            ireq.ProposedMaxServOutstandingCalled = new Integer16(10);
            ireq.ProposedDataStructureNestingLevel = new Integer8(5);

            mymmspdu.selectInitiate_RequestPDU(ireq);

            encoder.encode<MMSpdu>(mymmspdu, iecs.msMMSout);

            if (iecs.msMMSout.Length == 0)
            {
                iecs.logger.LogError("mms.SendInitiate: Encoding Error!");
                return -1;
            }

            this.Send(iecs, mymmspdu, 0, null);
            return 0;
        }

        private void Send(Iec61850State iecs, MMSpdu pdu, int InvokeIdInc, NodeBase[] OperationData)
        {
            if (iecs.CaptureDb.CaptureActive)
            {
                MMSCapture cap;
                iecs.msMMSout.Seek(0, SeekOrigin.Begin);
                iecs.msMMSout.Read(iecs.sendBuffer, 0, (int)iecs.msMMSout.Length);
                cap = new MMSCapture(iecs.sendBuffer, 0, iecs.msMMSout.Length, MMSCapture.CaptureDirection.Out);
                cap.MMSPdu = pdu;
                iecs.CaptureDb.AddPacket(cap);
            }
            insertCall(iecs, InvokeIdInc, OperationData);
            iecs.iso.Send(iecs);
        }

        private void AddIecAddress(Iec61850State iecs, string addr)
        {
            string[] parts = addr.Split(new char[] { '$' });
            NodeBase curld = iecs.DataModel.ied.GetActualChildNode();
            NodeBase curln, curfc; //, curdt;
            if (parts.Length < 1)
            {
                return;
            }
            curln = curld.AddChildNode(new NodeLN(parts[0]));
            if (parts.Length < 2)
            {
                return;
            }
            curfc = curln.AddChildNode(new NodeFC(parts[1]));
            if (parts.Length < 3)
            {
                return;
            }
        }

        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        public static void ConvertToUtcTime(DateTime dt, byte[] utc_time)
        {
            int t = (int)Scsm_MMS.ConvertToUnixTimestamp(dt);
            byte[] uib = BitConverter.GetBytes(t);
            utc_time[0] = uib[3];
            utc_time[1] = uib[2];
            utc_time[2] = uib[1];
            utc_time[3] = uib[0];

            UInt32 remainder = (UInt32)dt.Millisecond;
            UInt32 fractionOfSecond = (remainder) * 16777 + ((remainder * 216) / 1000);
            /* encode fraction of second */
            utc_time[4] = (byte)((fractionOfSecond >> 16) & 0xff);
            utc_time[5] = (byte)((fractionOfSecond >> 8) & 0xff);
            utc_time[6] = (byte)(fractionOfSecond & 0xff);
            /* encode time quality */
            utc_time[7] = 0x0a; /* 10 bit sub-second time accuracy */
        }


        public static DateTime ConvertFromUtcTime(byte[] utc_time, Object dataParam)
        {
            long seconds;
            long millis;

            if (utc_time != null && utc_time.Length == 8)
            {
                seconds = (utc_time[0] << 24) +
                          (utc_time[1] << 16) +
                          (utc_time[2] << 8) +
                          (utc_time[3]);

                millis = 0;
                for (int i = 0; i < 24; i++)
                {
                    if (((utc_time[(i / 8) + 4] << (i % 8)) & 0x80) > 0)
                    {
                        millis += 1000000 / (1 << (i + 1));
                    }
                }
                millis /= 1000;

                DateTime dt = ConvertFromUnixTimestamp(seconds);
                dt = dt.AddMilliseconds(millis);
                dataParam = utc_time[7];
                return dt.ToLocalTime();
            }
            else
            {
                dataParam = (byte)0xff;
                return DateTime.Now;
            }
        }

        public static DateTime ConvertFromUtcTime2(byte[] utc_time, Object dataParam)
        {
            long seconds;
            long fractionOfSecond;

            if (utc_time != null && utc_time.Length == 8)
            {
                seconds = (utc_time[0] << 24) +
                          (utc_time[1] << 16) +
                          (utc_time[2] << 8) +
                          (utc_time[3]);

                fractionOfSecond = (utc_time[4] << 16);
                fractionOfSecond += (utc_time[5] << 8);
                fractionOfSecond += (utc_time[6]);

                UInt32 millis = (UInt32)(fractionOfSecond / 16777);

                DateTime dt = ConvertFromUnixTimestamp(seconds);
                dt = dt.AddMilliseconds(millis);
                dataParam = utc_time[7];
                return dt.ToLocalTime();
            }
            else
            {
                dataParam = (byte)0xff;
                return DateTime.Now;
            }
        }

        private int insertCall(Iec61850State iecs, int InvokeIdInc, NodeBase[] OperationData)
        {
            if (iecs.OutstandingCalls.Count >= MaxCalls)
            {
                Logger.getLogger().LogWarning("Cannot send a request to client, " + MaxCalls + " calls pending for InvokeId " + (InvokeIdInc - 1).ToString());
                //return -1;
                // Auto Purge
                Logger.getLogger().LogWarning("Auto Purge Activated, deleting all previous operations. Too fast cycle of requests???");
                NodeBase[] ret;
                foreach (int id in iecs.OutstandingCalls.Keys)
                {
                    iecs.OutstandingCalls.TryRemove(id, out ret);
                    Logger.getLogger().LogWarning("Auto Purging Id=" + id + ", Operation data: " + ((ret == null) ? "null" : (ret[0] == null) ? "null" : ret[0].Name));
                }
            }
            if (!iecs.OutstandingCalls.TryAdd(InvokeIdInc - 1, OperationData)) return -2;
            return 0;
        }

        private NodeBase[] removeCall(Iec61850State iecs, int InvokeId)
        {
            NodeBase[] ret;
            if (!iecs.OutstandingCalls.TryRemove(InvokeId, out ret)) return null;
            return ret;
        }

        private void checkCalls(Iec61850State iecs, int InvokeId)
        {
            // If we do not believe we have caught all received ids
            NodeBase[] ret;
            foreach (int id in iecs.OutstandingCalls.Keys)
            {
                if (id + MaxCalls < InvokeId)
                    iecs.OutstandingCalls.TryRemove(id, out ret);
            }
        }

    }
}
