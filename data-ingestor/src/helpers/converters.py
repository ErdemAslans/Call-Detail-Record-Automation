from helpers.logger import main_logger as logger
import socket
import struct
from datetime import datetime, timezone
from models.cdrModel import CdrModel
from models.cdrModel import CallDirection
from models.cdrSubModels import Authorization, CallingParty, DateTime, Destination, DestinationCause, DestinationMediaCap, DestinationMediaTransportAddress, DestinationRsvp, DestinationVideoCap, DestinationVideoTransportAddress, DeviceMobile, FinalCalledParty, GlobalCall, HuntPilot, Incoming, LastRedirect, LastRedirecting, Orig, OrigCalledParty, OrigCause, OrigMediaCap, OrigRsvp, OrigVideoCap, OrigVideoTransportAddress, OriginalCalledParty, Outgoing, OutPulsed
from users import get_mongo_users_collection

def to_int(value):
    if value is None:
        return None
    try:
        return int(value)
    except ValueError as e:
        logger.warning(f"Error converting {value} to integer: {e}")
        return None

def convert_unix_time_toiso8601(unix_time):
    if unix_time is None or unix_time == 0:
        return None
    try:
        # UTC timezone ile dönüştür - MongoDB UTC olarak saklar
        return datetime.fromtimestamp(unix_time, tz=timezone.utc).isoformat()
    except (ValueError, OSError):
        logger.warning(f"Error converting {unix_time} to ISO8601")
        return None
def negative_int_to_ip(neg_ip):
    # 1. Negatif Değeri
    pos_ip = (1 << 32) + neg_ip

    # 2. Hexadecimal’e Dönüştürme
    hex_ip = f"{pos_ip:08X}"

    # 3. Byte Sırasını Ters Çevirme
    reversed_hex_ip = ''.join([hex_ip[i:i+2] for i in range(0, len(hex_ip), 2)][::-1])

    # 4. Ters Çevrilmiş Byte’ları Decimal’e Dönüştürme
    ip_parts = [str(int(reversed_hex_ip[i:i+2], 16)) for i in range(0, len(reversed_hex_ip), 2)]

    # 5. Sonuç IP Adresi
    return '.'.join(ip_parts)
    
def int_to_ip(ip):
    if ip is None or ip == 0:
        return None
    try:
        if ip < 0:
            return negative_int_to_ip(ip)
        return socket.inet_ntoa(struct.pack('!L', ip))
    except (struct.error, ValueError ) as e:
        logger.warning(f"Error converting {ip} to IP address, {e}")
        return None
    
def convert_boolean(value):
    if value is None:
        return None
    try:
        return bool(value)
    except ValueError as e:
        logger.warning(f"Error converting {value} to boolean: {e}")
        return None

def determine_call_direction(calling_party_number: str, original_called_party_number: str, users_collection) -> CallDirection:
    if calling_party_number in users_collection and original_called_party_number in users_collection:
        return CallDirection.INTERNAL
    elif calling_party_number not in users_collection:
        return CallDirection.INCOMING
    elif calling_party_number in users_collection and original_called_party_number not in users_collection:
        return CallDirection.OUTGOING
    return None

def parse_csv_to_model(csv_data: dict, users_collection) -> CdrModel:

    call_direction = determine_call_direction(
        csv_data.get('callingPartyNumber'),
        csv_data.get('originalCalledPartyNumber'),
        users_collection
    )
    return CdrModel(
        authCodeDescription = csv_data.get('authCodeDescription'),
        authorization = Authorization(
            code_value=csv_data.get('authorizationCodeValue'),
            level=to_int(csv_data.get('authorizationLevel')),
        ),
        callSecuredStatus = to_int(csv_data.get('callSecuredStatus')),
        calledPartyPatternUsage=to_int(csv_data.get('calledPartyPatternUsage')),
        callingParty=CallingParty(
            number=csv_data.get('callingPartyNumber'),
            partition=csv_data.get('callingPartyNumberPartition'),    
            unicode_login_user_id=csv_data.get('callingPartyUnicodeLoginUserID'),
            uri=csv_data.get('callingPartyNumber_uri'),
        ),
        cdrRecordType=to_int(csv_data.get('cdrRecordType')),
        clientMatterCode=csv_data.get('clientMatterCode'),
        comment=csv_data.get('comment'),
        currentRoutingReason=to_int(csv_data.get('currentRoutingReason')),
        dateTime=DateTime(
            connect=convert_unix_time_toiso8601(to_int(csv_data.get('dateTimeConnect'))),
            disconnect=convert_unix_time_toiso8601(to_int(csv_data.get('dateTimeDisconnect'))),
            origination=convert_unix_time_toiso8601(to_int(csv_data.get('dateTimeOrigination'))),
        ),
        destination=Destination(
            call_termination_on_behalf_of = to_int(csv_data.get('destCallTerminationOnBehalfOf')),
            conversation_id = csv_data.get('destConversationId'),
            device_name = csv_data.get('destDeviceName'),
            dtmf_method = csv_data.get('destDTMFMethod'),
            ip_addr = int_to_ip(to_int(csv_data.get('destIpAddr'))),
            ipv4v6_addr=csv_data.get('destIpv4v6Addr'),
            leg_identifier = to_int(csv_data.get('destLegIdentifier')),
            cause = DestinationCause(
                location = to_int(csv_data.get('destCause_location')),
                value = to_int(csv_data.get('destCause_value')),
            ),
            media_cap=DestinationMediaCap(
                bandwidth=to_int(csv_data.get('destMediaCap_Bandwidth')),
                bandwidth_channel2=to_int(csv_data.get('destMediaCap_Bandwidth_Channel2')),
                g723_bit_rate=to_int(csv_data.get('destMediaCap_g723BitRate')),
                max_frames_per_packet=to_int(csv_data.get('destMediaCap_maxFramesPerPacket')),
                payload_capability=to_int(csv_data.get('destMediaCap_payloadCapability')),
            ),
            media_transport_address=DestinationMediaTransportAddress(
                ip=int_to_ip(to_int(csv_data.get('destMediaTransportAddress_IP'))),
                port=to_int(csv_data.get('destMediaTransportAddress_Port')),
                ip_channel2=int_to_ip(to_int(csv_data.get('destMediaTransportAddress_IP_Channel2'))),
                port_channel2=to_int(csv_data.get('destMediaTransportAddress_Port_Channel2')),
            ),
            mobile=DeviceMobile(
                call_duration=to_int(csv_data.get('destMobileCallDuration')),
                device_name=csv_data.get('destMobileDeviceName'),
            ),
            node_id=to_int(csv_data.get('destNodeId')),
            precedence_level=to_int(csv_data.get('destPrecedenceLevel')),
            rsvp=DestinationRsvp(
                audio_stat=to_int(csv_data.get('destRSVPAudioStat')),
                video_stat=to_int(csv_data.get('destRSVPVideoStat')),
            ),
            span=to_int(csv_data.get('destSpan')),
            video_cap=DestinationVideoCap(
                bandwidth=to_int(csv_data.get('destVideoCap_Bandwidth')),
                codec=to_int(csv_data.get('destVideoCap_Codec')),
                resolution=to_int(csv_data.get('destVideoCap_Resolution')),
                bandwidth_channel2=to_int(csv_data.get('destVideoCap_Bandwidth_Channel2')),
                codec_channel2=to_int(csv_data.get('destVideoCap_Codec_Channel2')),
                resolution_channel2=to_int(csv_data.get('destVideoCap_Resolution_Channel2')),
            ),
            video_channel_role_channel2=to_int(csv_data.get('destVideoChannel_Role_Channel2')),
            video_transport_address=DestinationVideoTransportAddress(
                ip=int_to_ip(to_int(csv_data.get('destVideoTransportAddress_IP'))),
                port=to_int(csv_data.get('destVideoTransportAddress_Port')),
                ip_channel2=int_to_ip(to_int(csv_data.get('destVideoTransportAddress_IP_Channel2'))),
                port_channel2=to_int(csv_data.get('destVideoTransportAddress_Port_Channel2')),
            ),
        ),
        duration=to_int(csv_data.get('duration')),
        finalCalledParty=FinalCalledParty(
            number=csv_data.get('finalCalledPartyNumber'),
            partition=csv_data.get('finalCalledPartyNumberPartition'),
            unicode_login_user_id=csv_data.get('finalCalledPartyUnicodeLoginUserID'),
            uri=csv_data.get('finalCalledPartyNumber_uri'),
            pattern=csv_data.get('finalCalledPartyPattern'),
        ),
        finalMobileCalledPartyNumber=csv_data.get('finalMobileCalledPartyNumber'),
        globalCall=GlobalCall(
            cluster_id=csv_data.get('globalCallId_ClusterID'),
            call_id=csv_data.get('globalCallID_callId'),
            manager_id=to_int(csv_data.get('globalCallID_callManagerId')),
        ),
        huntPilot=HuntPilot(
            dn=csv_data.get('huntPilotDN'),
            partition=csv_data.get('huntPilotPartition'),
            pattern=csv_data.get('huntPilotPattern'),    
        ),
        incoming=Incoming(
            icid=csv_data.get('IncomingICID'),
            orig_ioi=csv_data.get('IncomingOrigIOI'),
            protocol_call_ref=csv_data.get('IncomingProtocolCallRef'),
            protocol_id=to_int(csv_data.get('IncomingProtocolID')),
            term_ioi=csv_data.get('IncomingTermIOI'),
        ),
        joinBehalfOf=to_int(csv_data.get('joinOnBehalfOf')),
        lastRedirect=LastRedirect(
            dn=csv_data.get('lastRedirectDn'),
            partition=csv_data.get('lastRedirectDnPartition'),
            reason=to_int(csv_data.get('lastRedirectRedirectReason')),
        ),
        lastRedirecting=LastRedirecting(
            party_pattern=csv_data.get('lastRedirectingPartyPattern'),
            routing_reason=to_int(csv_data.get('lastRedirectingRoutingReason')),
        ),
        mobileCallingPartyNumber=csv_data.get('mobileCallingPartyNumber'),
        mobileCallType=to_int(csv_data.get('mobileCallType')),
        orig=Orig(
            call_termination_on_behalf_of=to_int(csv_data.get('origCallTerminationOnBehalfOf')),
            call_party=OrigCalledParty(
                redirect_on_behalf_of=to_int(csv_data.get('origCalledPartyRedirectOnBehalfOf')),
                redirect_reason=to_int(csv_data.get('origCalledPartyRedirectReason')),
            ),
            cause=OrigCause(
                location=to_int(csv_data.get('origCause_location')),
                value=to_int(csv_data.get('origCause_value')),    
            ),
            conversation_id=to_int(csv_data.get('origConversationId')),
            device_name=csv_data.get('origDeviceName'),
            dtmf_method=to_int(csv_data.get('origDTMFMethod')),
            ipv4v6_addr=csv_data.get('origIpv4v6Addr'),
            ip_addr=int_to_ip(to_int(csv_data.get('origIpAddr'))),
            leg_call_identifier=to_int(csv_data.get('origLegCallIdentifier')),
            node_id=to_int(csv_data.get('origNodeId')),
            orig_media_cap=OrigMediaCap(
                bandwidth=to_int(csv_data.get('origMediaCap_Bandwidth')),
                g723_bit_rate=to_int(csv_data.get('origMediaCap_g723BitRate')),
                max_frames_per_packet=to_int(csv_data.get('origMediaCap_maxFramesPerPacket')),
                payload_capability=to_int(csv_data.get('origMediaCap_payloadCapability')),
            ),
            precedence_level=to_int(csv_data.get('origPrecedenceLevel')),
            routing_reason=to_int(csv_data.get('origRoutingReason')),
            video_channel_role_channel2=to_int(csv_data.get('origVideoChannel_Role_Channel2')),
            rsvp=OrigRsvp(
                audio_stat=to_int(csv_data.get('origRSVPAudioStat')),
                video_stat=to_int(csv_data.get('origRSVPVideoStat')), 
            ),
            span=csv_data.get('origSpan'),
            video_cap=OrigVideoCap(
                bandwidth=to_int(csv_data.get('origVideoCap_Bandwidth')),
                codec=to_int(csv_data.get('origVideoCap_Codec')),
                resolution=to_int(csv_data.get('origVideoCap_Resolution')),
                bandwidth_channel2=to_int(csv_data.get('origVideoCap_Bandwidth_Channel2')),
                codec_channel2=to_int(csv_data.get('origVideoCap_Codec_Channel2')),
                resolution_channel2=to_int(csv_data.get('origVideoCap_Resolution_Channel2')),
            ),
            video_transport_address=OrigVideoTransportAddress(
                ip=int_to_ip(to_int(csv_data.get('origVideoTransportAddress_IP'))),
                port=to_int(csv_data.get('origVideoTransportAddress_Port')),
                ip_channel2=int_to_ip(to_int(csv_data.get('origVideoTransportAddress_IP_Channel2'))),
                port_channel2=to_int(csv_data.get('origVideoTransportAddress_Port_Channel2')),
            ),
        ),
        originalCalledParty=OriginalCalledParty(
            number=csv_data.get('originalCalledPartyNumber'),
            partition=csv_data.get('originalCalledPartyNumberPartition'),
            uri=csv_data.get('originalCalledPartyNumber_uri'),
            pattern=csv_data.get('originalCalledPartyPattern'),
        ),
        outgoing=Outgoing(
            icid=csv_data.get('OutgoingICID'),
            orig_ioi=csv_data.get('OutgoingOrigIOI'),
            protocol_call_ref=csv_data.get('OutgoingProtocolCallRef'),
            protocol_id=to_int(csv_data.get('OutgoingProtocolID')),
            term_ioi=csv_data.get('OutgoingTermIOI'),
        ),
        outpulsed=OutPulsed(
            calling_party_number=csv_data.get('outpulsedCallingPartyNumber'),
            called_party_number=csv_data.get('outpulsedCalledPartyNumber'),
            last_redirecting_number=csv_data.get('outpulsedLastRedirectingNumber'),
            original_called_party_number=csv_data.get('outpulsedOriginalCalledPartyNumber'),
        ),
        totalWaitTimeInQueue=to_int(csv_data.get('totalWaitTimeInQueue')),
        wasCallQueued=convert_boolean(to_int(csv_data.get('wasCallQueued'))),
        callDirection=call_direction.value,
        pk_id=csv_data.get('pkid'),
    )