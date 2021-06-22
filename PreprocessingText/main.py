####includes
import psycopg2
import re
import requests

import io
from urllib.request import urlopen
from bs4 import BeautifulSoup

import os
import nltk
from nltk.corpus import brown

import numpy as np
import pymystem3

####GLOBAL VARIABLES
mystem = pymystem3.Mystem()
#######

texts=[]
#counter=0

def formatingText(url,id):
    global texts#глобальная переменная
    path='Texts'

    html = urlopen(url).read()
    soup = BeautifulSoup(html, features="html.parser")
    for script in soup(["script", "style"]):
        script.extract()    # убираем скрипты и css-стили
    text = soup.get_text()
    ############################
    lines = (line.strip() for line in text.splitlines())#разделение текста на строки
    chunks = (phrase.strip() for line in lines for phrase in line.split("  "))#разделение строк на куски текста
    text = '\n'.join(chunk for chunk in chunks if (chunk and len(chunk)>15))##добавление строк в text, если фраза длинее 15 символов    
    ###################################
    #######GETTING STOP-LIST#########
    #################################
    stop_words_ru= set(nltk.corpus.stopwords.words('russian'))
    stop_words_en= set(nltk.corpus.stopwords.words('english'))#добавить АНГ.ЯЗ
    ######################################
    #########FORMATING THE FILES##########
    ######################################
    word=nltk.word_tokenize(text)# токинезация текста i-го документа
    word_ws=[w.lower()  for w in   word if w.isalpha() ]#нижний регистр
    word_w=[w for w in word_ws if w not in stop_words_ru ]#исключение слов и символов

    lem = mystem.lemmatize ((" ").join(word_w))# лемматизация текста
    lema=[w for w in lem if w.isalpha() and len(w)>1]
    freq=nltk.FreqDist(lema)# распределение слов в i -м документе по частоте
        
    z=[] #обновление списка для нового документа
    z=[(key+":"+str(val)) for key,val in freq.items() if val>1] # частота упоминания через : от слова
    #texts.append("|text" +" "+(" ").join(z)+"\n")
    texts.append(( id  , ("|text" +" "+(" ").join(z)+"\n") ,    'russian'   ))
    ######################

def textGetterFromDB():  
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
    cursor = conn.cursor()
    #cursor.execute('SELECT id, url	FROM public.cs_rawdata)')
    cursor.execute('SELECT id, url	FROM public.cs_rawdata WHERE id NOT IN (SELECT id_cs FROM public.preprocessing)')
    records = cursor.fetchall()
    

    counter=0
    #########GETTING TEXT FROM HTML
    for row in records:
        if(counter>30):
            break
        try:
            url = row[1]
            id= row[0]
            formatingText(url,id)
            print("Fetched text "+str(counter))
        except Exception as ex:
            print(ex)
        counter+=1
    ######################################

    #################
    #SendDATAtoDB
    ###############
    global texts
    
    for tup in texts:
        insert_query = 'insert into public.preprocessing (id_cs, b_text, language) values {}'.format(tup)
        cursor.execute(insert_query)
        #print (cursor.mogrify(insert_query).decode('utf8'))

    conn.commit()
    cursor.close()
    conn.close()
    #######################


if __name__ == '__main__':
    textGetterFromDB()
    #themeModeling()

