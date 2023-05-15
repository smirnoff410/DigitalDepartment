from typing import Union

from fastapi import FastAPI
# Beat tracking example
import librosa

existFileData = [ [ 1, "Eminem – Mockingbird.mp3"], [ 2, "miyagi-andy-panda-minor-mp3.mp3"] ]

existTrackData = [ [99.38401442307692, "Miagy and Andy Panda"], [112.34714673913044, "Eminem"] ]

app = FastAPI()

#Изменить путь на свое локальное хранилище изображений
storage_path = "D:\Projects\DigitalDepartment\Audio\storage"

@app.get("/{track_id}")
def read_root(track_id: int):
    # 1. Get the file path to an included audio example
    #filename = filename = librosa.example('nutcracker')

    filename = ""
    for i in range(len(existFileData)):
        if(existFileData[i][0] == track_id):
            filename = existFileData[i][1]

    # 2. Load the audio as a waveform `y`
    #    Store the sampling rate as `sr`
    y, sr = librosa.load(storage_path + "\\" + filename)

    # 3. Run the default beat tracker
    tempo, beat_frames = librosa.beat.beat_track(y=y, sr=sr)

    print('Estimated tempo: {:.2f} beats per minute'.format(tempo))

    resultTrackName = ""
    for i in range(len(existTrackData)):
        if(existTrackData[i][0] == tempo):
            resultTrackName = existTrackData[i][1]

    # 4. Convert the frame indices of beat events into timestamps
    #beat_times = librosa.frames_to_time(beat_frames, sr=sr)

    #print(beat_times)
    similarTracks = get_similar_tracks(tempo)
    return {"TrackName": resultTrackName, "Similar": similarTracks}

def get_similar_tracks(tempo: int):

    result = []
    for i in range(len(existTrackData)):
        if(existTrackData[i][0] >= tempo - 15 and existTrackData[i][0] <= tempo + 15):
            result.append(existTrackData[i][1])
    return result