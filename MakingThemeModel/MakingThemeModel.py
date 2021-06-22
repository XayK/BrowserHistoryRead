import artm
import psycopg2
import nltk
import numpy as np
import pymystem3
#VARIABLES
top_tokens=[]
id_of_css=[]
#varibale to rem
id_of_req=0

###########################
#bigartm - тематическое моделирование
################################
def themeModeling():
    print('magic\n')
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
                        password='password', host='192.168.56.129')
    cursor = conn.cursor()

    requesORIG=""
    time_p=""
    global id_of_req
    #
    # считываем задание на 
    #
    #cursor.execute('SELECT id_of_req ,name_disp, text_of_disp, time_of_para   FROM public.requests INNER JOIN public.discipline ON requests.id_disp=discipline.id_disp WHERE id_of_req NOT IN (SELECT id_req FROM public.output_usefull)')
    cursor.execute('SELECT id_of_req ,name_disp, text_of_disp, time_of_para   FROM public.requests INNER JOIN public.discipline ON requests.id_disp=discipline.id_disp WHERE id_of_req NOT IN (SELECT id_req FROM public.output_usefull)')
    recordsGET=cursor.fetchall()
    for leng in range(len(recordsGET)):
        for row in recordsGET:
            requesORIG=row[2]
            time_p=row[3]
            id_of_req=row[0]
            break#выход после первой!!!!!!!   
        #
        #####format the original text
        ##########################
        mystem = pymystem3.Mystem()

        lines = (line.strip() for line in requesORIG.splitlines())#разделение текста на строки
        chunks = (phrase.strip() for line in lines for phrase in line.split("  "))#разделение строк на куски текста
        requesORIG = '\n'.join(chunk for chunk in chunks if (chunk and len(chunk)>15))##добавление строк в reques, если фраза длинее 15 символов    

        stop_words_ru= set(nltk.corpus.stopwords.words('russian'))
        stop_words_en= set(nltk.corpus.stopwords.words('english'))#добавить АНГ.ЯЗ

        word=nltk.word_tokenize(requesORIG)# токинезация текста i-го документа
        word_ws=[w.lower()  for w in   word if w.isalpha() ]#нижний регистр
        word_w=[w for w in word_ws if w not in stop_words_ru ]#исключение слов и символов

        lem = mystem.lemmatize ((" ").join(word_w))# лемматизация текста
        lema=[w for w in lem if w.isalpha() and len(w)>1]
        freq=nltk.FreqDist(lema)# распределение слов в i -м документе по частоте
        
        z=[] #обновление списка для нового документа
        z=[(key+":"+str(val)) for key,val in freq.items() if val>1] # частота упоминания через : от слова
        requesORIG=("|text" +" "+(" ").join(z)+"\n")
        #########################
        ########################
    
        str_for_time=''
        t=time_p.split(' ')
        for i in range(len(t)-2):
            if(i!=0):
                str_for_time+= 'or '
            if(t[i][1]==1 and t[i][3]==1 and t[i][5]==1):
                str_for_time+= '(last_visit_time<'+ '2021-01-04 10:35:00' + ' and last_visit_time>' +'2021-01-04 09:00:00) '#время первной недели и первой пары в понедельник 9 января
            elif(t[i][1]==1 and t[i][3]==1 and t[i][5]==2):
                str_for_time+= '(last_visit_time<'+ '2021-01-04 10:45:00' + ' and last_visit_time>' +'2021-01-04 12:20:00) '#время первной недели и 2 пары в понедельник 9 января
            # и далее

        #cursor.execute('SELECT preprocessing.id, b_text,id_cs	FROM public.preprocessing INNER JOIN public.cs_rawdata ON preprocessing.id_cs=cs_rawdata.id  WHERE preprocessing.id_cs NOT IN (SELECT id_cs FROM public.output_usefull) and ('+str_for_time+')')
        #cursor.execute('SELECT preprocessing.id, b_text,id_cs	FROM public.preprocessing INNER JOIN public.cs_rawdata ON preprocessing.id_cs=cs_rawdata.id')
        cursor.execute('SELECT preprocessing.id, b_text,id_cs	FROM public.preprocessing INNER JOIN public.cs_rawdata ON preprocessing.id_cs=cs_rawdata.id')# !!!добавить фильтр по time_p
        records = cursor.fetchall()
        import io
        
        fO=io.open('datasORIG.txt','w',encoding='utf8')
        fO.write(requesORIG)# добавляем на анализ ИСХОДНЫЙ ТЕКСТВ
        fO.close()

        import os
        if not (os.path.exists('modelsOfBA')):
            os.mkdir("modelsOfBA")
        if not (os.path.exists('modelsOfBAORIG')):
            os.mkdir("modelsOfBAORIG")

    
        batch_vectorizerO = artm.BatchVectorizer(data_path='datasORIG.txt',# путь к "мешку слов"
                                               data_format='vowpal_wabbit',# формат данных
                                               target_folder='modelsOfBAORIG', #папка с частотной матрицей из batch
                                               batch_size=1)# количество документов водном batch
        dictionaryO = artm.Dictionary(data_path='modelsOfBAORIG')# загрузка данных в словарь

        modelO = artm.ARTM(num_topics=1,
                         num_document_passes=10,#10 проходов по документу
                         dictionary=dictionaryO,
                         scores=[artm.TopTokensScore(name='top_tokens_score')])
        modelO.fit_offline(batch_vectorizer=batch_vectorizerO, num_collection_passes=10)#10 проходов по коллекции
    
        top_tokensO = modelO.score_tracker['top_tokens_score']
        #################################
        for i in range(0,len(records)):
            global id_of_css
            f=io.open('datas.txt','w',encoding='utf8')
            toWrite=records[i][1][0:-2]+'\n'
            f.write(toWrite)
            id_of_css.append(records[i][2])
            f.close()

            # создание частотной матрицы из batch
            batch_vectorizer = artm.BatchVectorizer(data_path='datas.txt',# путь к "мешку слов"
                                                    data_format='vowpal_wabbit',# формат данных
                                                   target_folder='modelsOfBA', #папка с частотной матрицей из batch
                                                    batch_size=1)# количество документов водном batch
            dictionary = artm.Dictionary(data_path='modelsOfBA')# загрузка данных в словарь
            model = artm.ARTM(num_topics=1,
                             num_document_passes=10,#10 проходов по документу
                             dictionary=dictionary,
                             scores=[artm.TopTokensScore(name='top_tokens_score')])
            model.fit_offline(batch_vectorizer=batch_vectorizer, num_collection_passes=10)#10 проходов по коллекции
            top_tokens = model.score_tracker['top_tokens_score']

            checkwithOriginal(top_tokens,model,top_tokensO,modelO,i)
        #############################################
        ###############################################


#usefullSites=[]
################################################################
##################Сравнение с исходным документом################
################################################################
def checkwithOriginal(top_tokens,model,top_tokensO,modelO,i):
    # пока исходный документ последний
    global id_of_css

    #for i in range(1,len(model.topic_names)-1):
    #for i in range(0,len(model.topic_names)):
    counter=0
    for (token, weight) in zip(top_tokens.last_tokens[model.topic_names[0]],
             top_tokens.last_weights[model.topic_names[0]]):    
             ####сравнение 
        for (tokenO, weightO) in zip(top_tokensO.last_tokens[modelO.topic_names[-1]],
             top_tokensO.last_weights[modelO.topic_names[-1]]):    
            if token==tokenO:
                counter+=1
    conn = psycopg2.connect(dbname='db_urls', user='postgres', 
        password='password', host='192.168.56.129')
    cursor = conn.cursor()
    cursor.execute('INSERT INTO public.output_usefull (percentage, id_req, id_cs) VALUES ('+ str(counter/10*100) +', '+ str(id_of_req) +', '+ str(id_of_css[i]) +' )')
    conn.commit()
    cursor.close()
    conn.close()
    #################################################################

if __name__ == '__main__':
    themeModeling()
    