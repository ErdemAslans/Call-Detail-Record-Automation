from pydantic import BaseModel, Field
from typing import Optional
from datetime import datetime


class Authorization(BaseModel):
    level: Optional[int] = Field(None, title="Authorization Level")
    code_value: Optional[str] = Field(None, title="Authorization Code Value")


class CallingParty(BaseModel):
    number: Optional[str] = Field(None, title="Calling Party Number")
    partition: Optional[str] = Field(None, title="Calling Party Partition")
    uri: Optional[str] = Field(None, title="Calling Party URI")
    unicode_login_user_id: Optional[str] = Field(None, title="Calling Party Unicode Login User ID")


class DateTime(BaseModel):
    connect: Optional[datetime] = Field(None, title="Connect Time")
    disconnect: Optional[datetime] = Field(None, title="Disconnect Time")
    origination: Optional[datetime] = Field(None, title="Origination Time")


class DestinationCause(BaseModel):
    location: Optional[int] = Field(None, title="Destination Cause Location")
    value: Optional[int] = Field(None, title="Destination Cause Value")


class DestinationMediaCap(BaseModel):
    bandwidth: Optional[int] = Field(None, title="Destination Media Bandwidth")
    bandwidth_channel2: Optional[int] = Field(None, title="Destination Media Bandwidth Channel 2")
    g723_bit_rate: Optional[int] = Field(None, title="Destination Media G723 Bit Rate")
    max_frames_per_packet: Optional[int] = Field(None, title="Destination Media Max Frames Per Packet")
    payload_capacity: Optional[int] = Field(None, title="Destination Media Payload Capability")


class DestinationMediaTransportAddress(BaseModel):
    ip: Optional[str] = Field(None, title="Destination Media Transport Address IP")
    port: Optional[int] = Field(None, title="Destination Media Transport Address Port")
    ip_channel2: Optional[str] = Field(None, title="Destination Media Transport Address IP Channel 2")
    port_channel2: Optional[int] = Field(None, title="Destination Media Transport Address Port Channel 2")


class DeviceMobile(BaseModel):
    call_duration: Optional[int] = Field(None, title="Call Duration")
    device_name: Optional[str] = Field(None, title="Device Name")


class DestinationRsvp(BaseModel):
    audio_stat: Optional[int] = Field(None, title="Audio Status")
    video_stat: Optional[int] = Field(None, title="Video Status")


class DestinationVideoCap(BaseModel):
    bandwidth: Optional[int] = Field(None, title="Destination Video Bandwidth")
    bandwidth_channel2: Optional[int] = Field(None, title="Destination Video Bandwidth Channel 2")
    codec: Optional[int] = Field(None, title="Destination Video Codec")
    codec_channel2: Optional[int] = Field(None, title="Destination Video Codec Channel 2")
    resolution: Optional[int] = Field(None, title="Destination Video Resolution")
    resolution_channel2: Optional[int] = Field(None, title="Destination Video Resolution Channel 2")


class DestinationVideoTransportAddress(BaseModel):
    ip: Optional[str] = Field(None, title="Destination Video Transport Address IP")
    ip_channel2: Optional[str] = Field(None, title="Destination Video Transport Address IP Channel 2")
    port: Optional[int] = Field(None, title="Destination Video Transport Address Port")
    port_channel2: Optional[int] = Field(None, title="Destination Video Transport Address Port Channel 2")


class Destination(BaseModel):
    call_termination_on_behalf_of: Optional[int] = Field(None, title="Call Termination On Behalf Of")
    conversation_id: Optional[str] = Field(None, title="Conversation ID")
    dtmf_method: Optional[str] = Field(None, title="DTMF Method")
    device_name: Optional[str] = Field(None, title="Device Name")
    ip_addr: Optional[str] = Field(None, title="Destination IP Address")
    ipv4v6_addr: Optional[str] = Field(None, title="Destination IPv4v6 Address")
    leg_identifier: Optional[int] = Field(None, title="Leg Identifier")
    cause: Optional[DestinationCause] = Field(None, title="Destination Cause")
    media_cap: Optional[DestinationMediaCap] = Field(None, title="Destination Media Capability")
    media_transport_address: Optional[DestinationMediaTransportAddress] = Field(None, title="Destination Media Transport Address")
    mobile: Optional[DeviceMobile] = Field(None, title="Device Mobile")
    node_id: Optional[int] = Field(None, title="Node ID")
    precedence_level: Optional[int] = Field(None, title="Precedence Level")
    rsvp: Optional[DestinationRsvp] = Field(None, title="Destination RSVP")
    span: Optional[int] = Field(None, title="Destination Span")
    video_cap: Optional[DestinationVideoCap] = Field(None, title="Destination Video Capability")
    video_channel_role_channel2: Optional[int] = Field(None, title="Destination Video Channel Role Channel 2")
    video_transport_address: Optional[DestinationVideoTransportAddress] = Field(None, title="Destination Video Transport Address")


class FinalCalledParty(BaseModel):
    number: Optional[str] = Field(None, title="Final Called Party Number")
    partition: Optional[str] = Field(None, title="Final Called Party Partition")
    uri: Optional[str] = Field(None, title="Final Called Party URI")
    pattern: Optional[str] = Field(None, title="Final Called Party Pattern")
    unicode_login_user_id: Optional[str] = Field(None, title="Final Called Party Unicode Login User ID")


class GlobalCall(BaseModel):
    call_id: Optional[str] = Field(None, title="GlobalCall ID")
    manager_id: Optional[int] = Field(None, title="GlobalCall Manager ID")
    cluster_id: Optional[str] = Field(None, title="GlobalCall Cluster ID")


class HuntPilot(BaseModel):
    dn: Optional[str] = Field(None, title="Hunt Pilot DN")
    partition: Optional[str] = Field(None, title="Hunt Pilot Partition")
    pattern: Optional[str] = Field(None, title="Hunt Pilot Pattern")


class Incoming(BaseModel):
    icid: Optional[str] = Field(None, title="Incoming Call ID")
    orig_ioi: Optional[str] = Field(None, title="Incoming Call Orig IOI")
    protocol_call_ref: Optional[str] = Field(None, title="Incoming Call Protocol Call Reference")
    protocol_id: Optional[int] = Field(None, title="Incoming Call Protocol ID")
    term_ioi: Optional[str] = Field(None, title="Incoming Call Term IOI")


class LastRedirect(BaseModel):
    dn: Optional[str] = Field(None, title="Last Redirect DN")
    dn_partition: Optional[str] = Field(None, title="Last Redirect DN Partition")
    uri: Optional[str] = Field(None, title="Last Redirect URI")
    redirect_on_behalf_of: Optional[int] = Field(None, title="Last Redirect On Behalf Of")
    reason: Optional[int] = Field(None, title="Last Redirect Reason")


class LastRedirecting(BaseModel):
    party_pattern: Optional[str] = Field(None, title="Last Redirecting Party Pattern")
    routing_reason: Optional[int] = Field(None, title="Last Redirecting Routing Reason")


class OrigCalledParty(BaseModel):
    redirect_on_behalf_of: Optional[int] = Field(None, title="Origination Call Party Redirect On Behalf Of")
    redirect_reason: Optional[int] = Field(None, title="Origination Call Party Redirect Reason")


class OrigCause(BaseModel):
    location: Optional[int] = Field(None, title="Origination Cause Location")
    value: Optional[int] = Field(None, title="Origination Cause Value")


class OrigMediaCap(BaseModel):
    bandwidth: Optional[int] = Field(None, title="Origination Media Bandwidth")
    bandwidth_channel2: Optional[int] = Field(None, title="Origination Media Bandwidth Channel 2")
    g723_bit_rate: Optional[int] = Field(None, title="Origination Media G723 Bit Rate")
    max_frames_per_packet: Optional[int] = Field(None, title="Origination Media Max Frames Per Packet")
    payload_capability: Optional[int] = Field(None, title="Origination Media Payload Capability")


class OrigMediaTransportAddress(BaseModel):
    ip: Optional[str] = Field(None, title="Origination Media Transport Address IP")
    port: Optional[int] = Field(None, title="Origination Media Transport Address Port")


class OrigMobile(BaseModel):
    call_duration: Optional[int] = Field(None, title="Origination Call Duration")
    device_name: Optional[str] = Field(None, title="Origination Device Name")


class OrigRsvp(BaseModel):
    audio_stat: Optional[int] = Field(None, title="Origination Audio Status")
    video_stat: Optional[int] = Field(None, title="Origination Video Status")


class OrigVideoCap(BaseModel):
    bandwidth: Optional[int] = Field(None, title="Origination Video Bandwidth")
    bandwidth_channel2: Optional[int] = Field(None, title="Origination Video Bandwidth Channel 2")
    codec: Optional[int] = Field(None, title="Origination Video Codec")
    codec_channel2: Optional[int] = Field(None, title="Origination Video Codec Channel 2")
    resolution: Optional[int] = Field(None, title="Origination Video Resolution")
    resolution_channel2: Optional[int] = Field(None, title="Origination Video Resolution Channel 2")


class OrigVideoTransportAddress(BaseModel):
    ip: Optional[str] = Field(None, title="Origination Video Transport Address IP")
    ip_channel2: Optional[str] = Field(None, title="Origination Video Transport Address IP Channel 2")
    port: Optional[int] = Field(None, title="Origination Video Transport Address Port")
    port_channel2: Optional[int] = Field(None, title="Origination Video Transport Address Port Channel 2")


class Orig(BaseModel):
    call_termination_on_behalf_of: Optional[int] = Field(None, title="Origination Call Termination On Behalf Of")
    call_party: Optional[OrigCalledParty] = Field(None, title="Origination Call Party")
    cause: Optional[OrigCause] = Field(None, title="Origination Cause")
    conversation_id: Optional[int] = Field(None, title="Origination Conversation ID")
    dtmf_method: Optional[int] = Field(None, title="Origination DTMF Method")
    device_name: Optional[str] = Field(None, title="Origination Device Name")
    ip_addr: Optional[str] = Field(None, title="Origination IP Address")
    ipv4v6_addr: Optional[str] = Field(None, title="Origination IPv4v6 Address")
    leg_call_identifier: Optional[int] = Field(None, title="Origination Leg Call Identifier")
    orig_media_cap: Optional[OrigMediaCap] = Field(None, title="Origination Media Capability")
    node_id: Optional[int] = Field(None, title="Origination Node ID")
    precedence_level: Optional[int] = Field(None, title="Origination Precedence Level")
    rsvp: Optional[OrigRsvp] = Field(None, title="Origination RSVP")
    routing_reason: Optional[int] = Field(None, title="Origination Routing Reason")
    span: Optional[int] = Field(None, title="Origination Span")
    video_cap: Optional[OrigVideoCap] = Field(None, title="Origination Video Capability")
    video_transport_address: Optional[OrigVideoTransportAddress] = Field(None, title="Origination Video Transport Address")
    video_channel_role_channel2: Optional[int] = Field(None, title="Origination Video Channel Role Channel 2")


class OriginalCalledParty(BaseModel):
    number: Optional[str] = Field(None, title="Original Called Party Number")
    partition: Optional[str] = Field(None, title="Original Called Party Partition")
    uri: Optional[str] = Field(None, title="Original Called Party URI")
    pattern: Optional[str] = Field(None, title="Original Called Party Pattern")


class Outgoing(BaseModel):
    icid: Optional[str] = Field(None, title="Outgoing Call ID")
    orig_ioi: Optional[str] = Field(None, title="Outgoing Call Orig IOI")
    protocol_call_ref: Optional[str] = Field(None, title="Outgoing Call Protocol Call Reference")
    protocol_id: Optional[int] = Field(None, title="Outgoing Call Protocol ID")
    term_ioi: Optional[str] = Field(None, title="Outgoing Call Term IOI")


class OutPulsed(BaseModel):
    called_party_number: Optional[str] = Field(None, title="Outgoing Pulsed Called Party Number")
    calling_party_number: Optional[str] = Field(None, title="Outgoing Pulsed Calling Party Number")
    last_redirecting_number: Optional[str] = Field(None, title="Outgoing Pulsed Last Redirecting Number")
    original_called_party_number: Optional[str] = Field(None, title="Outgoing Pulsed Original Called Party Number")