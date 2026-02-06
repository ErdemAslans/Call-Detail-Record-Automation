from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime
from models.cdrSubModels import Authorization, CallingParty, DateTime, Destination, FinalCalledParty, GlobalCall, HuntPilot, Incoming, LastRedirect, LastRedirecting, Orig, OriginalCalledParty, Outgoing, OutPulsed
from enum import Enum

class CallDirection(Enum):
    INCOMING = 1
    OUTGOING = 2
    INTERNAL = 3

class CdrModel(BaseModel):
    authCodeDescription:Optional[str] = Field(None, title="Auth Code Description")
    authorization:Optional[Authorization] = Field(None, title="Authorization")
    callSecuredStatus:Optional[int] = Field(None, title="Call Securd Status")
    calledPartyPatternUsage: Optional[int] = Field(None, title="Called Party Pattern Usage")
    callingParty: Optional[CallingParty] = Field(None, title="Calling Party")
    cdrRecordType: Optional[int] = Field(None, title="CDR Record Type")
    clientMatterCode: Optional[str] = Field(None, title="Client Matter Code")
    comment: Optional[str] = Field(None, title="Comment")
    currentRoutingReason: Optional[int] = Field(None, title="Current Routing Reason")
    dateTime: Optional[DateTime] = Field(None, title="Date Time")
    destination: Optional[Destination] = Field(None, title="Destination")
    duration: Optional[int] = Field(None, title="Duration")
    finalCalledParty: Optional[FinalCalledParty] = Field(None, title="Final Called Party Number")
    finalMobileCalledPartyNumber: Optional[str] = Field(None, title="Final Mobile Called Party Number")
    globalCall: Optional[GlobalCall] = Field(None, title="Global Call")
    huntPilot: Optional[HuntPilot] = Field(None, title="Hunt Pilot")
    incoming: Optional[Incoming] = Field(None, title="Incoming")
    joinBehalfOf: Optional[int] = Field(None, title="Join Behalf Of")
    lastRedirect: Optional[LastRedirect] = Field(None, title="Last Redirect")
    lastRedirecting: Optional[LastRedirecting] = Field(None, title="Last Redirecting")
    mobileCallingPartyNumber: Optional[str] = Field(None, title="Mobile Calling Party Number")
    mobileCallType: Optional[int] = Field(None, title="Mobile Call Type")
    orig: Optional[Orig] = Field(None, title="Orig")
    originalCalledParty: Optional[OriginalCalledParty] = Field(None, title="Original Called Party")
    outgoing: Optional[Outgoing] = Field(None, title="Outgoing")
    outpulsed: Optional[OutPulsed] = Field(None, title="Outpulsed")
    wasCallQueued: Optional[bool] = Field(None, title="Was Call Queued")
    totalWaitTimeInQueue: Optional[int] = Field(None, title="Total Wait Time In Queue")
    pk_id: Optional[str] = Field(None, title="Primary Key ID")
    callDirection: Optional[int] = Field(None, title="Call Direction")