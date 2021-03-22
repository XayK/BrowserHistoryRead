

#Подключение и скачивание базы ссылок на сайты
import psycopg2
import re
import requests

conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
cursor = conn.cursor()
cursor.execute('SELECT id, url	FROM public.rawdata')
records = cursor.fetchall()

pathS='Texts'
#Words=[]
counter=0
import io

for row in records:
    #########GETTING TEXT FROM HTML
    from urllib.request import urlopen
    from bs4 import BeautifulSoup
    url = row[1]
    html = urlopen(url).read()
    soup = BeautifulSoup(html, features="html.parser")
    for script in soup(["script", "style"]):
        script.extract()    # rip it out
    text = soup.get_text()

    lines = (line.strip() for line in text.splitlines())#разделение текста на строки
    chunks = (phrase.strip() for line in lines for phrase in line.split("  "))#разделение строк на куски текста
    text = '\n'.join(chunk for chunk in chunks if (chunk and len(chunk)>15))##добавление строк в text, если фраза длинее 15 символов

    #f = requests.get(row[1])
    #f = f.text
    ##f= re.sub('<[^>]*>', '', f)
    #f= re.sub('{[^>]*}', '', f)
    #f= re.sub('\n', ' ', f)
    ###########################
    counter+=1
    fo=io.open(pathS+'/'+str(counter)+'.txt','w',encoding='utf8')
    fo.write(text)
    fo.close()
    #Words.append({})
    #for word in f.split():
    #    if(Words[-1].get(word)==None):
    #       Words[-1][word]=1
    #    else:
    #        Words[-1][word]=Words[-1].get(word)+1
    #print(Words[-1])

cursor.close()
conn.close()

############################################################
#########Добавление исходного документа в файл##############
############################################################

from urllib.request import urlopen
from bs4 import BeautifulSoup
url = 'https://ru.wikipedia.org/wiki/%D0%91%D0%B0%D0%B7%D0%B0_%D0%B4%D0%B0%D0%BD%D0%BD%D1%8B%D1%85'
html = urlopen(url).read()
soup = BeautifulSoup(html, features="html.parser")
for script in soup(["script", "style"]):
    script.extract()    # rip it out
textOriginal = soup.get_text()

lines = (line.strip() for line in text.splitlines())#разделение текста на строки
chunks = (phrase.strip() for line in lines for phrase in line.split("  "))#разделение строк на куски текста
textOriginal = '\n'.join(chunk for chunk in chunks if (chunk and len(chunk)>15))##добавление строк в text, если фраза длинее 15 символов
counter+=1
fo=io.open(pathS+'/'+str(counter)+'.txt','w',encoding='utf8')
fo.write(textOriginal)
fo.close()

#########################
#########################
#########################

#лемминга/стемминг


#подготовка к ТМ


###################################
#######GETTING STOP-LIST#########
#################################

import nltk
from nltk.corpus import brown
stop_words= set(nltk.corpus.stopwords.words('russian'))
#print (stop_words)

######################################
#########FORMATING THE FILES##########
######################################

#import matplotlib.pyplot as plt
import os
import nltk
import numpy as np

import pymystem3
mystem = pymystem3.Mystem()

path='Texts'
import io
f=io.open('datas.txt','w',encoding='utf8')
x=[];y=[]; s=[]


for i in range(1,len(os.listdir(path))+1): #перебор файлов с документами по номерам i
      filename=path+'/'+str(i)+".txt"
      text=" "
      with io.open(filename, encoding = 'UTF-8') as file_object:# сбор текста из файла i-го документа
            for line in file_object:
                  if len(line)!=0:
                        text=text+" "+line
      word=nltk.word_tokenize(text)# токинезация текста i-го документа
      word_ws=[w.lower()  for w in   word if w.isalpha() ]#исключение слов и символов
      word_w=[w for w in word_ws if w not in stop_words ]#нижний регистр
      lem = mystem . lemmatize ((" ").join(word_w))# лемматизация i -го документа
      lema=[w for w in lem if w.isalpha() and len(w)>1]
      freq=nltk.FreqDist(lema)# распределение слов в i -м документе по частоте
      z=[]# обновление списка для нового документа
      z=[(key+":"+str(val)) for key,val in freq.items() if val>1] # частота упоминания через : от слова
      f.write("|text" +" "+(" ").join(z)+'\n')# запись в мешок слов с меткой |text
      c=[];d=[]
      for key,val in freq.items():#подготовка к сортировке слов по убыванию частоты в i -м документе
            if val>1:
                  c.append(val); d.append(key)
      a=[];b=[]
      for k in np.arange(0,len(c),1):#сортировка слов по убыванию частоты в i -м документе
            ind=c.index(max(c));  a.append(c[ind])
            b.append(d[ind]); del c[ind]; del d[ind]
      x.append(i)#список номеров документов
      y.append(len(a))#список количества слов в документах
      a=a[0:10];b=b[0:10]# TOP-10 для частот a  и слов b в i -м документе
      y_pos = np.arange(1,len(a)+1,1)#построение TOP-10 диаграмм
      performance =a
      #plt.barh(y_pos, a)
      #plt.yticks(y_pos, b)
      #plt.xlabel(u'Количество слов')
      #plt.title(u'Частоты слов в документе № %i'%i, size=12)
      #plt.grid(True)
      #plt.show()
#plt.title(u'Количество слов в документах', size=12)
#plt.xlabel(u'Номера документов', size=12)
#plt.ylabel(u'Количество слов', size=12)
#plt.bar(x,y, 1)
#plt.grid(True)
#plt.show()
f.close()

###########################
#bigartm - тематическое моделирование
################################


import artm
# создание частотной матрицы из batch
batch_vectorizer = artm.BatchVectorizer(data_path='datas.txt',# путь к "мешку слов"
                                        data_format='vowpal_wabbit',# формат данных
                                       target_folder='habrahabr', #папка с частотной матрицей из batch
                                        batch_size=len(os.listdir(path))+1)# количество документов водном batch

dictionary = artm.Dictionary(data_path='habrahabr')# загрузка данных в словарь
model = artm.ARTM(num_topics=len(os.listdir(path))+1,
                  num_document_passes=10,#10 проходов по документу
                  dictionary=dictionary,
                  scores=[artm.TopTokensScore(name='top_tokens_score')])
model.fit_offline(batch_vectorizer=batch_vectorizer, num_collection_passes=10)#10 проходов по коллекции
top_tokens = model.score_tracker['top_tokens_score']



for topic_name in model.topic_names:
    print (topic_name)
    for (token, weight) in zip(top_tokens.last_tokens[topic_name],
                               top_tokens.last_weights[topic_name]):    
         print (token, '-', round(weight,3))





################################################################
##################Сравнение с исходным документом################
################################################################
# пока исходный документ последний
usefullSites=[]
for i in range(1,len(model.topic_names)-1):
    counter=0
    for (token, weight) in zip(top_tokens.last_tokens[model.topic_names[i]],
                               top_tokens.last_weights[model.topic_names[i]]):    
         ####сравнение 
         for (tokenO, weightO) in zip(top_tokens.last_tokens[model.topic_names[-1]],
                               top_tokens.last_weights[model.topic_names[-1]]):    
             if token==tokenO:
                counter+=1
    print("coincidence for ",model.topic_names[i]," is ", counter/10, "%")
    usefullSites.append(counter)


#################################################################


################################################################
##################Определение полезных документов################
################################################################
conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
cursor = conn.cursor()
cursor.execute('SELECT id, url,visit_count	FROM public.rawdata')
records = cursor.fetchall()
for id in range(len(usefullSites)):
    if(usefullSites[id]>1 and records[id][2]>1):#хороший сайт если много совпадающий популярных слов и частая посещаемость студентами
        print(records[id][1], " is usefull site")
cursor.close()
conn.close()