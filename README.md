# GlobosatTrackParser

I wrote this tool to analyze the track files generated by [GloBoSaT](https://github.com/Gbertaz/GloBoSaT) to make sure that the writing speed on SD card is enough to keep up with the GPS update rate. 

### Identifies any lost fixes and fills the gap left by any GPS signal loss by linearly interpolating the data.

Useful as a *debugging tool* but also as a tool to generate any missing data to create videos with telemetry data in overlay.


# Usage

The first parameter is the GPS update rate in Hertz with which the track has been logged. It only works when [GloBoSaT](https://github.com/Gbertaz/GloBoSaT) is set on **Tracking mode Time**

```
GlobosatTrackParser.exe <GPS update rate Hz> <Input file path>

Ex: 

GlobosatTrackParser.exe 5 "C:\Tracks\data_36.log"
```

**The resulting output file will be named "data_36_out.log"**


***

# Output

### Outputs a text file which contains the original data plus the following calculated values:

* Lean angle
    * This is an attempt to roughly calculate the lean angle of a motorcycle given the speed and the radius of the turn
* Distance
* Total time
* Trip time
* Average speed


It also calculates and display the following statistics:

* Max speed
* Min speed
* Average speed
* Max altitude
* Min altitude
* Max temperature
* Min temperature
* Average temperature
* Max heart rate bpm
* Min heart rate bpm
* Average heart rate bpm
* Total time
* Moving time
* Total distance

Output example:

![Output](https://github.com/Gbertaz/GlobosatTrackParser/blob/master/images/output.PNG)

**It might be useful to understand where a fix has been lost, uncommenting the two lines of code in FixCounter.cs lines 52 and 53 will output the GPS fix that follows the lost one**

***

### GPS data loss could also be caused by a momentary loss of the GPS signal:
* the antenna does not have an optimal view of the sky (trees, buildings, etc...)
* driving in a tunnel


# Future developments

I would like to completely re-write it as a web application that allows the users to upload the track files and create an history.
Render the track on a map, see the statistics at any given point of the track.
