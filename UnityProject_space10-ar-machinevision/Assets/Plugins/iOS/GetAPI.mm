/***********************************************************
* Copyright (C) 2018 6degrees.xyz Inc.
*
* This file is part of the 6D.ai Beta SDK and is not licensed
* for commercial use.
*
* The 6D.ai Beta SDK can not be copied and/or distributed without
* the express permission of 6degrees.xyz Inc.
*
* Contact developers@6d.ai for licensing requests.
***********************************************************/

extern "C" {
    void GetAPIKey(char* apiKey, int bufferSize)
    {
        NSString* plistFile = [[NSBundle mainBundle] pathForResource:@"SixDegreesSDK" ofType:@"plist"];
        if (plistFile) 
        {
            NSDictionary *plistDict = [NSDictionary dictionaryWithContentsOfFile:plistFile];
            if (plistDict) 
            {
                id dictApiKey = [plistDict valueForKey:@"SIXDEGREES_API_KEY"];
                if (dictApiKey && [dictApiKey isKindOfClass:[NSString class]]) 
                {
                    strcpy(apiKey, [dictApiKey UTF8String]);
                }
            }
        }
    }
}
