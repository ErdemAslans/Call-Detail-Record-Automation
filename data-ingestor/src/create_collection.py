import asyncio
from helpers.logger import main_logger as logger
from pymongo import MongoClient
from helpers.config import load_config

config = load_config()

async def create_collection_if_not_exists():
    client = MongoClient(config['mongo']['uri'])
    db = client[config['mongo']['database']]
    collection_name = config['mongo']['collection']
    
    if collection_name not in db.list_collection_names():
        db.create_collection(collection_name)
        schema = {
            'bsonType': 'object',
            'required': ['cdrRecordType'],
            'properties': {
                'authCodeDescription': {'bsonType': ['string', 'null']},
                'authorization': {
                    'bsonType': 'object',
                    'properties': {
                        'level': {'bsonType': ['int', 'null']},
                        'code_value': {'bsonType': ['string', 'null']}
                    }
                },
                'callSecuredStatus': {'bsonType': ['int', 'null']},
                'calledPartyPatternUsage': {'bsonType': ['int', 'null']},
                'callingParty': {
                    'bsonType': 'object',
                    'properties': {
                        'number': {'bsonType': ['string', 'null']},
                        'partition': {'bsonType': ['string', 'null']},
                        'uri': {'bsonType': ['string', 'null']},
                        'unicode_login_user_id': {'bsonType': ['string', 'null']}
                    }
                },
                'cdrRecordType': {'bsonType': ['int', 'null']},
                'clientMatterCode': {'bsonType': ['string', 'null']},
                'comment': {'bsonType': ['string', 'null']},
                'currentRoutingReason': {'bsonType': ['int', 'null']},
                'dateTime': {
                    'bsonType': 'object',
                    'properties': {
                        'connect': {'bsonType': ['date', 'null']},
                        'disconnect': {'bsonType': ['date', 'null']},
                        'origination': {'bsonType': ['date', 'null']}
                    }
                },
                'destination': {
                    'bsonType': 'object',
                    'properties': {
                        'call_termination_on_behalf_of': {'bsonType': ['int', 'null']},
                        'conversation_id': {'bsonType': ['string', 'null']},
                        'dtmf_method': {'bsonType': ['string', 'null']},
                        'device_name': {'bsonType': ['string', 'null']},
                        'ip_addr': {'bsonType': ['string', 'null']},
                        'ipv4v6_addr': {'bsonType': ['string', 'null']},
                        'leg_identifier': {'bsonType': ['int', 'null']},
                        'cause': {
                            'bsonType': 'object',
                            'properties': {
                                'location': {'bsonType': ['int', 'null']},
                                'value': {'bsonType': ['int', 'null']}
                            }
                        },
                        'media_cap': {
                            'bsonType': 'object',
                            'properties': {
                                'bandwidth': {'bsonType': ['int', 'null']},
                                'bandwidth_channel2': {'bsonType': ['int', 'null']},
                                'g723_bit_rate': {'bsonType': ['int', 'null']},
                                'max_frames_per_packet': {'bsonType': ['int', 'null']},
                                'payload_capacity': {'bsonType': ['int', 'null']}
                            }
                        },
                        'media_transport_address': {
                            'bsonType': 'object',
                            'properties': {
                                'ip': {'bsonType': ['string', 'null']},
                                'port': {'bsonType': ['int', 'null']},
                                'ip_channel2': {'bsonType': ['string', 'null']},
                                'port_channel2': {'bsonType': ['int', 'null']}
                            }
                        },
                        'mobile': {
                            'bsonType': 'object',
                            'properties': {
                                'call_duration': {'bsonType': ['int', 'null']},
                                'device_name': {'bsonType': ['string', 'null']}
                            }
                        },
                        'node_id': {'bsonType': ['int', 'null']},
                        'precedence_level': {'bsonType': ['int', 'null']},
                        'rsvp': {
                            'bsonType': 'object',
                            'properties': {
                                'audio_stat': {'bsonType': ['int', 'null']},
                                'video_stat': {'bsonType': ['int', 'null']}
                            }
                        },
                        'span': {'bsonType': ['int', 'null']},
                        'video_cap': {
                            'bsonType': 'object',
                            'properties': {
                                'bandwidth': {'bsonType': ['int', 'null']},
                                'bandwidth_channel2': {'bsonType': ['int', 'null']},
                                'codec': {'bsonType': ['int', 'null']},
                                'codec_channel2': {'bsonType': ['int', 'null']},
                                'resolution': {'bsonType': ['int', 'null']},
                                'resolution_channel2': {'bsonType': ['int', 'null']}
                            }
                        },
                        'video_channel_role_channel2': {'bsonType': ['int', 'null']},
                        'video_transport_address': {
                            'bsonType': 'object',
                            'properties': {
                                'ip': {'bsonType': ['string', 'null']},
                                'ip_channel2': {'bsonType': ['string', 'null']},
                                'port': {'bsonType': ['int', 'null']},
                                'port_channel2': {'bsonType': ['int', 'null']}
                            }
                        }
                    }
                },
                'duration': {'bsonType': ['int', 'null']},
                'finalCalledParty': {
                    'bsonType': 'object',
                    'properties': {
                        'number': {'bsonType': ['string', 'null']},
                        'partition': {'bsonType': ['string', 'null']},
                        'uri': {'bsonType': ['string', 'null']},
                        'pattern': {'bsonType': ['string', 'null']},
                        'unicode_login_user_id': {'bsonType': ['string', 'null']}
                    }
                },
                'finalMobileCalledPartyNumber': {'bsonType': ['string', 'null']},
                'globalCall': {
                    'bsonType': 'object',
                    'properties': {
                        'call_id': {'bsonType': ['string', 'null']},
                        'manager_id': {'bsonType': ['int', 'null']},
                        'cluster_id': {'bsonType': ['string', 'null']}
                    }
                },
                'huntPilot': {
                    'bsonType': 'object',
                    'properties': {
                        'dn': {'bsonType': ['string', 'null']},
                        'partition': {'bsonType': ['string', 'null']},
                        'pattern': {'bsonType': ['string', 'null']}
                    }
                },
                'incoming': {
                    'bsonType': 'object',
                    'properties': {
                        'icid': {'bsonType': ['string', 'null']},
                        'orig_ioi': {'bsonType': ['string', 'null']},
                        'protocol_call_ref': {'bsonType': ['string', 'null']},
                        'protocol_id': {'bsonType': ['int', 'null']},
                        'term_ioi': {'bsonType': ['string', 'null']}
                    }
                },
                'joinBehalfOf': {'bsonType': ['int', 'null']},
                'lastRedirect': {
                    'bsonType': 'object',
                    'properties': {
                        'dn': {'bsonType': ['string', 'null']},
                        'dn_partition': {'bsonType': ['string', 'null']},
                        'uri': {'bsonType': ['string', 'null']},
                        'redirect_on_behalf_of': {'bsonType': ['int', 'null']},
                        'reason': {'bsonType': ['int', 'null']}
                    }
                },
                'lastRedirecting': {
                    'bsonType': 'object',
                    'properties': {
                        'party_pattern': {'bsonType': ['string', 'null']},
                        'routing_reason': {'bsonType': ['int', 'null']}
                    }
                },
                'mobileCallingPartyNumber': {'bsonType': ['string', 'null']},
                'mobileCallType': {'bsonType': ['int', 'null']},
                'orig': {
                    'bsonType': 'object',
                    'properties': {
                        'call_termination_on_behalf_of': {'bsonType': ['int', 'null']},
                        'call_party': {
                            'bsonType': 'object',
                            'properties': {
                                'redirect_on_behalf_of': {'bsonType': ['int', 'null']},
                                'redirect_reason': {'bsonType': ['int', 'null']}
                            }
                        },
                        'cause': {
                            'bsonType': 'object',
                            'properties': {
                                'location': {'bsonType': ['int', 'null']},
                                'value': {'bsonType': ['int', 'null']}
                            }
                        },
                        'conversation_id': {'bsonType': ['int', 'null']},
                        'dtmf_method': {'bsonType': ['int', 'null']},
                        'device_name': {'bsonType': ['string', 'null']},
                        'ip_addr': {'bsonType': ['string', 'null']},
                        'ipv4v6_addr': {'bsonType': ['string', 'null']},
                        'leg_call_identifier': {'bsonType': ['int', 'null']},
                        'orig_media_cap': {
                            'bsonType': 'object',
                            'properties': {
                                'bandwidth': {'bsonType': ['int', 'null']},
                                'bandwidth_channel2': {'bsonType': ['int', 'null']},
                                'g723_bit_rate': {'bsonType': ['int', 'null']},
                                'max_frames_per_packet': {'bsonType': ['int', 'null']},
                                'payload_capability': {'bsonType': ['int', 'null']}
                            }
                        },
                        'node_id': {'bsonType': ['int', 'null']},
                        'precedence_level': {'bsonType': ['int', 'null']},
                        'rsvp': {
                            'bsonType': 'object',
                            'properties': {
                                'audio_stat': {'bsonType': ['int', 'null']},
                                'video_stat': {'bsonType': ['int', 'null']}
                            }
                        },
                        'routing_reason': {'bsonType': ['int', 'null']},
                        'span': {'bsonType': ['int', 'null']},
                        'video_cap': {
                            'bsonType': 'object',
                            'properties': {
                                'bandwidth': {'bsonType': ['int', 'null']},
                                'bandwidth_channel2': {'bsonType': ['int', 'null']},
                                'codec': {'bsonType': ['int', 'null']},
                                'codec_channel2': {'bsonType': ['int', 'null']},
                                'resolution': {'bsonType': ['int', 'null']},
                                'resolution_channel2': {'bsonType': ['int', 'null']}
                            }
                        },
                        'video_channel_role_channel2': {'bsonType': ['int', 'null']},
                        'video_transport_address': {
                            'bsonType': 'object',
                            'properties': {
                                'ip': {'bsonType': ['string', 'null']},
                                'ip_channel2': {'bsonType': ['string', 'null']},
                                'port': {'bsonType': ['int', 'null']},
                                'port_channel2': {'bsonType': ['int', 'null']}
                            }
                        }
                    }
                },
                'originalCalledParty': {
                    'bsonType': 'object',
                    'properties': {
                        'number': {'bsonType': ['string', 'null']},
                        'partition': {'bsonType': ['string', 'null']},
                        'uri': {'bsonType': ['string', 'null']},
                        'pattern': {'bsonType': ['string', 'null']}
                    }
                },
                'outgoing': {
                    'bsonType': 'object',
                    'properties': {
                        'icid': {'bsonType': ['string', 'null']},
                        'orig_ioi': {'bsonType': ['string', 'null']},
                        'protocol_call_ref': {'bsonType': ['string', 'null']},
                        'protocol_id': {'bsonType': ['int', 'null']},
                        'term_ioi': {'bsonType': ['string', 'null']}
                    }
                },
                'outpulsed': {
                    'bsonType': 'object',
                    'properties': {
                        'called_party_number': {'bsonType': ['string', 'null']},
                        'calling_party_number': {'bsonType': ['string', 'null']},
                        'last_redirecting_number': {'bsonType': ['string', 'null']},
                        'original_called_party_number': {'bsonType': ['string', 'null']}
                    }
                },
                'wasCallQueued': {'bsonType': ['bool', 'null']},
                'totalWaitTimeInQueue': {'bsonType': ['int', 'null']},
                'callDirection': {'bsonType': ['int', 'null']},
            }
        }
        db.command({
            'collMod': collection_name,
            'validator': {'$jsonSchema': schema},
            'validationLevel': 'strict',
            'validationAction': 'error'
        })
        logger.info(f"Collection '{collection_name}' created with schema validation.")
    else:
        logger.info(f"Collection '{collection_name}' already exists.")

if __name__ == '__main__':
    asyncio.run(create_collection_if_not_exists())