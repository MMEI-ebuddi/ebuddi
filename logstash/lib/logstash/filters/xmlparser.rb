# encoding: utf-8
require "logstash/filters/base"
require "logstash/namespace"
require "logstash/timestamp"
require "json"


# Add any asciidoc formatted documentation here
# This example filter will replace the contents of the default
# message field with whatever you specify in the configuration.
#
# It is only intended to be used as an example.
class LogStash::Filters::XmlParser < LogStash::Filters::Base

  # Setting the config_name here is required. This is how you
  # configure this filter from your Logstash config.
  #
  # filter {
  #   example { message => "My message..." }
  # }
  config_name "xmlparser"

  # Replace the message with this value.
  config :target, :validate => :string, :default => "message"
  config :source, :validate => :string, :default => "@message"
  config :ts_field, :validate => :string, :default => "date_time"


  public
  def register
    require "nokogiri"
    require "xmlsimple"
    # Add instance variables
  end # def register


  def processSession(v)


      correctedAnswers = Array.new
      wrongdAnswers = Array.new
      correctedActions = Array.new 
      wrongedActions = Array.new 

      allActions = Array.new
      quizQuestions = Array.new


      run = v['run'] 


      noOfRuns = run.size


      totalTimeForRun = 0

      run.each do |r|
        actions = r['actions']
        rno = r['number']
        begin
          actions = actions[0]['action']
        rescue Exception => e
          actions = []
        end

        quizQuestions = r['quizQuestions']
        begin
          quizQuestions = quizQuestions[0]['question']
        rescue Exception => e
          quizQuestions = []
        end

        quizQuestions = quizQuestions.nil? ? [] : quizQuestions
        actions = actions.nil? ? [] : actions

        



        actions.each do |a|
          answer = a['answer']
          expected = a['expected']
          a['run_no'] = rno;

          totalTime = Integer(a['totalTime'])

          if totalTime > totalTimeForRun
            totalTimeForRun = totalTime
          end


          if answer == expected
            correctedActions.push(answer)
          else
            wrongedActions.push(answer)
          end
        end

        allActions << actions

        

      end

      quizQuestions.each do |q|
        answer = q['answers']
        expected = q['correctAnswer']
        text = q['text']

        if answer == expected
          correctedAnswers.push(text)
        else
          wrongdAnswers.push(text)
        end
      end





      v.delete('run')


      # v["actions"] = allActions
      ## v["correctActions"] = correctActions
      # v["quizQuestions"] = quizQuestions

      wrongedActions.uniq!
      correctedActions.uniq!
      correctedAnswers.uniq!
      wrongdAnswers.uniq!

      # v["wrongedActions"] = wrongedActions
      # v["correctedActions"] = correctedActions
      # v["correctedAnswers"] = correctedAnswers
      # v["wrongdAnswers"] = wrongdAnswers
      v["totalTimeForRun"] = totalTimeForRun
      v["noOfRuns"] = noOfRuns
      v["wrongAnswersCount"] = wrongdAnswers.length
      v["wrongActionsCount"] = wrongedActions.length

      unless v["latitude"].nil?
        v["location"] = { lat: v["latitude"] , lon: v["longitude"] }
        v.delete("latitude")
        v.delete("longitude")
      end

      v["location"] = { lat: 8.488317 , lon: -9.771055 }
      

      

  end


  public
  def filter(event)
    xmlparsed = event[@source]
    begin
    

      session = xmlparsed['session']
    
      timestamp = nil

      session0 = session.nil? ? nil : session[0]
      session1 = session.nil? ? nil : (session.size >0  ? session[1] : nil) 

      sessions = Array.new




      unless session0.nil? 
        processSession(session0)
        sessions << session0
      end

      unless session1.nil? 
        processSession(session1)
        sessions << session1
      end

      timestamp =  session0.nil?  ? "" :  session0['date']




      
      user = xmlparsed["User"][0]

      event['session'] = sessions
      event['user'] = user
      event['userName'] = user["name"]
      event['userSex'] = user["sex"]
      event['userAge'] = Integer(user["age"])

      event['userId'] = user["uniqueUserId"] || user["name"]
      event['mobilePhoneNumber'] = user["mobilePhoneNumber"] || "+447971988645"
      event['correctedAnswersRun0'] = session0.nil? ? [] : session0["correctedAnswers"]
      event['correctedAnswersRun1'] = session1.nil? ? [] :  session1["correctedAnswers"]


      event['totalTimeForRunScene1'] = session0.nil? ? 0 :  session0["totalTimeForRun"]
      event['totalTimeForRunScene2'] = session1.nil? ? 0 :  session1["totalTimeForRun"]

      event['noOfRunsScene1'] = session0.nil? ? 0 :  session0["noOfRuns"]
      event['noOfRunsScene2'] = session1.nil? ? 0 :  session1["noOfRuns"]
      # event["_id"] = timestamp.gsub(" ","_") +  "~!" + user["uniqueUserId"]
      event[@ts_field] = timestamp  
    rescue Exception => e

      event.cancel
      puts "event canceled, error #{event} #{e}"

      # raise e
      
    end

    


    filter_matched(event)
  end # def filter

end # class LogStash::Filters::Example